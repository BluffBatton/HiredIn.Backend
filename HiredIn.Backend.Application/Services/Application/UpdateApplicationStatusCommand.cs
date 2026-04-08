using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ApplicationDTOs;
using HiredIn.Backend.Contracts.DTOs.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Application
{
    public class UpdateApplicationStatusCommand : IRequest
    {
        public Guid ApplicationId { get; set; }
        public ApplicationStatusUpdateDTO Status { get; set; }

        public UpdateApplicationStatusCommand(Guid applicationId, ApplicationStatusUpdateDTO status)
        {
            ApplicationId = applicationId;
            Status = status;
        }
    }

    public class UpdateApplicationStatusCommandHandler : IRequestHandler<UpdateApplicationStatusCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public UpdateApplicationStatusCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(UpdateApplicationStatusCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var application = await _context.Applications
                .Include(a => a.Vacancy)
                .FirstOrDefaultAsync(a =>
                    a.Id == request.ApplicationId &&
                    a.DeletedAtUtc == null,
                    cancellationToken);

            if (application == null)
                throw new KeyNotFoundException("Application not found.");

            var membership = await _context.CompanyMembers
                .FirstOrDefaultAsync(cm =>
                    cm.CompanyId == application.Vacancy.CompanyId &&
                    cm.UserId == currentUserId.Value,
                    cancellationToken);

            if (membership == null)
                throw new UnauthorizedAccessException("You do not belong to this company.");

            if (membership.Role is not (Domain.Enums.CompanyMemberRole)CompanyMemberRole.Owner and
                not (Domain.Enums.CompanyMemberRole)CompanyMemberRole.Recruiter)
                throw new UnauthorizedAccessException("You do not have permission to update application status.");

            var newStatus = (HiredIn.Backend.Domain.Enums.ApplicationStatus)request.Status.Status;

            if (application.Status == HiredIn.Backend.Domain.Enums.ApplicationStatus.Accepted)
                throw new InvalidOperationException("Accepted application cannot be changed.");

            if (application.Status == HiredIn.Backend.Domain.Enums.ApplicationStatus.Rejected)
                throw new InvalidOperationException("Rejected application cannot be changed.");

            if (newStatus == HiredIn.Backend.Domain.Enums.ApplicationStatus.Submitted)
                throw new InvalidOperationException("Cannot change application status back to Submitted.");

            application.Status = newStatus;
            application.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
