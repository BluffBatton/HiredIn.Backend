using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainVacancy = HiredIn.Backend.Domain.Entities.Vacancy;

namespace HiredIn.Backend.Application.Services.Vacancy
{
    public class CreateVacancyCommand : IRequest<Guid>
    {
        public VacancyCreateDTO Vacancy { get; set; }

        public CreateVacancyCommand(VacancyCreateDTO vacancy)
        {
            Vacancy = vacancy;
        }
    }

    public class CreateVacancyCommandHandler : IRequestHandler<CreateVacancyCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public CreateVacancyCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<Guid> Handle(CreateVacancyCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var membership = await _context.CompanyMembers
                .FirstOrDefaultAsync(cm =>
                    cm.CompanyId == request.Vacancy.CompanyId &&
                    cm.UserId == currentUserId.Value,
                    cancellationToken);

            if (membership == null)
                throw new UnauthorizedAccessException("You do not belong to this company.");

            if (membership.Role != CompanyMemberRole.Owner &&
                membership.Role != CompanyMemberRole.Recruiter)
                throw new UnauthorizedAccessException("You do not have permission to create vacancies.");

            var companyExists = await _context.Companies
                .AnyAsync(c => c.Id == request.Vacancy.CompanyId && c.DeletedAtUtc == null, cancellationToken);

            if (!companyExists)
                throw new KeyNotFoundException("Company not found.");

            var vacancy = _mapper.Map<DomainVacancy>(request.Vacancy);
            vacancy.CreatedAtUtc = DateTime.UtcNow;
            vacancy.UpdatedAtUtc = DateTime.UtcNow;

            await _context.Vacancies.AddAsync(vacancy, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return vacancy.Id;
        }
    }
}