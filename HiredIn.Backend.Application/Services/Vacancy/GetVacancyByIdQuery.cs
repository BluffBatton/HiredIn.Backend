using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.VacancyDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Vacancy
{
    public class GetVacancyByIdQuery : IRequest<VacancyReadDTO>
    {
        public Guid VacancyId { get; set; }

        public GetVacancyByIdQuery(Guid vacancyId)
        {
            VacancyId = vacancyId;
        }
    }

    public class GetVacancyByIdQueryHandler : IRequestHandler<GetVacancyByIdQuery, VacancyReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public GetVacancyByIdQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<VacancyReadDTO> Handle(GetVacancyByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var vacancy = await _context.Vacancies
                .Include(v => v.Company)
                .FirstOrDefaultAsync(v =>
                    v.Id == request.VacancyId &&
                    v.DeletedAtUtc == null,
                    cancellationToken);

            if (vacancy == null)
                throw new KeyNotFoundException("Vacancy not found.");

            var hasAccess = await _context.CompanyMembers
                .AnyAsync(cm =>
                    cm.CompanyId == vacancy.CompanyId &&
                    cm.UserId == currentUserId.Value,
                    cancellationToken);

            if (!hasAccess)
                throw new UnauthorizedAccessException("You do not have access to this vacancy.");

            return _mapper.Map<VacancyReadDTO>(vacancy);
        }
    }
}