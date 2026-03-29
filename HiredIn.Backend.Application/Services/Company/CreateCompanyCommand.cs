using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyDTOs;
using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Company
{
    public class CreateCompanyCommand : IRequest
    {
        public CompanyCreateDTO Company { get; set; }

        public CreateCompanyCommand(CompanyCreateDTO company)
        {
            Company = company;
        }
    }

    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public CreateCompanyCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == currentUserId.Value, cancellationToken);

            if (currentUser == null)
                throw new KeyNotFoundException("User not found.");



            var company = _mapper.Map<Domain.Entities.Company>(request.Company);

            company.CreatedAtUtc = DateTime.UtcNow;
            company.UpdatedAtUtc = DateTime.UtcNow;

            await _context.Companies.AddAsync(company, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var companyMember = new CompanyMember
            {
                CompanyId = company.Id,
                UserId = currentUserId.Value,
                Role = (Domain.Enums.CompanyMemberRole)CompanyMemberRole.Owner,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _context.CompanyMembers.AddAsync(companyMember, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return;
        }
    }
}