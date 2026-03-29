using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.CompanyDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Company
{
    public class UpdateMyCompanyCommand : IRequest
    {
        public CompanyUpdateDTO Dto { get; set; }
        public UpdateMyCompanyCommand(CompanyUpdateDTO dto)
        {
            Dto = dto;
        }
    }

    public class UpdateMyCompanyCommandHandler : IRequestHandler<UpdateMyCompanyCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public UpdateMyCompanyCommandHandler(IApplicationDbContext context, IMapper mapper, IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }


        public async Task Handle(UpdateMyCompanyCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is logged out");
            }

            var companyMember = await _context.CompanyMembers
                .FirstOrDefaultAsync(cm => cm.UserId == userId, cancellationToken);

            if(companyMember == null)
            {
                throw new UnauthorizedAccessException("User is not a member of any company");
            }
            else if (companyMember.Role == Domain.Enums.CompanyMemberRole.Recruiter)
            {
                throw new UnauthorizedAccessException("Restricted access for user's role");
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == companyMember.CompanyId, cancellationToken);
            if(company == null)
            {
                throw new UnauthorizedAccessException("Company doesn't exists");
            }

            var updatedCompany = _mapper.Map(request.Dto, company);

            updatedCompany.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}