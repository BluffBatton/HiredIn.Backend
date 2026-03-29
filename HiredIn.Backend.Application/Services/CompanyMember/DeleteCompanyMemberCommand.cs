using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.CompanyMember
{
    public class DeleteCompanyMemberCommand : IRequest
    {
        public Guid CompanyMemberId { get; set; }

        public DeleteCompanyMemberCommand(Guid companyMemberId)
        {
            CompanyMemberId = companyMemberId;
        }
    }

    public class DeleteCompanyMemberCommandHandler : IRequestHandler<DeleteCompanyMemberCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteCompanyMemberCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(DeleteCompanyMemberCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var targetMember = await _context.CompanyMembers
                .FirstOrDefaultAsync(cm => cm.Id == request.CompanyMemberId, cancellationToken);

            if (targetMember == null)
                throw new KeyNotFoundException("Company member not found.");

            var currentMembership = await _context.CompanyMembers
                .FirstOrDefaultAsync(
                    cm => cm.CompanyId == targetMember.CompanyId &&
                          cm.UserId == currentUserId.Value,
                    cancellationToken);

            if (currentMembership == null)
                throw new UnauthorizedAccessException("You do not belong to this company.");

            if (currentMembership.Role != CompanyMemberRole.Owner)
                throw new UnauthorizedAccessException("You do not have permission to remove company members.");

            if (targetMember.Role == CompanyMemberRole.Owner)
            {
                var ownersCount = await _context.CompanyMembers
                    .CountAsync(
                        cm => cm.CompanyId == targetMember.CompanyId &&
                              cm.Role == CompanyMemberRole.Owner,
                        cancellationToken);

                if (ownersCount <= 1)
                    throw new InvalidOperationException("Cannot remove the last owner of the company.");
            }

            _context.CompanyMembers.Remove(targetMember);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}