using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Application
{
    public class WithdrawApplicationCommand : IRequest
    {
        public Guid ApplicationId { get; set; }

        public WithdrawApplicationCommand(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
    }

    public class WithdrawApplicationCommandHandler : IRequestHandler<WithdrawApplicationCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public WithdrawApplicationCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(WithdrawApplicationCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(cp => cp.UserId == currentUserId.Value, cancellationToken);

            if (candidateProfile == null)
                throw new KeyNotFoundException("Candidate profile not found.");

            var application = await _context.Applications
                .Include(a => a.Resume)
                .FirstOrDefaultAsync(a =>
                    a.Id == request.ApplicationId &&
                    a.DeletedAtUtc == null,
                    cancellationToken);

            if (application == null)
                throw new KeyNotFoundException("Application not found.");

            if (application.Resume.CandidateProfileId != candidateProfile.Id)
                throw new UnauthorizedAccessException("You do not have access to this application.");

            if ((Contracts.DTOs.Enums.ApplicationStatus)application.Status == ApplicationStatus.Accepted)
                throw new InvalidOperationException("Accepted application cannot be withdrawn.");

            application.DeletedAtUtc = DateTime.UtcNow;
            application.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
