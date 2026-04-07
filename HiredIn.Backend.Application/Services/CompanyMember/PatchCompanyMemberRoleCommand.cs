using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CompanyMember
{
    public class PatchCompanyMemberRoleCommand : IRequest
    {
        public Guid Id { get; set; }
        public CompanyMemberRole? Role { get; set; }
        public PatchCompanyMemberRoleCommand(Guid id, CompanyMemberRole role)
        {
            Id = id;
            Role = role;
        }
    }

    public class PatchCompanyMemberRoleCommandHandler : IRequestHandler<PatchCompanyMemberRoleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        public PatchCompanyMemberRoleCommandHandler(IApplicationDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(PatchCompanyMemberRoleCommand request, CancellationToken cancellationToken)
        {
            if(request.Role == CompanyMemberRole.Owner)
            {
                throw new UnauthorizedAccessException("Can't change role to owner");
            }
            if(!request.Role.HasValue)
            {
                throw new ArgumentNullException(nameof(request.Role));
            }

            var userId = _userContextService.GetCurrentUserId();

            var masterCompanyMember = await _context.CompanyMembers
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (masterCompanyMember is not null && masterCompanyMember.Role != Domain.Enums.CompanyMemberRole.Owner)
            {
                throw new UnauthorizedAccessException("Recruiter can't change role of other members in company");
            }


            var companyMember = await _context.CompanyMembers.FindAsync(new object[] { request.Id }, cancellationToken);
            if (companyMember is null)
            {
                throw new KeyNotFoundException($"Company member with id '{request.Id}' not found.");
            }

            companyMember.Role = (Domain.Enums.CompanyMemberRole)request.Role.Value;
            
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
