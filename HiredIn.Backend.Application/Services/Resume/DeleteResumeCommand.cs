using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Resume
{
    public class DeleteResumeCommand : IRequest
    {
        public Guid ResumeId { get; set; }

        public DeleteResumeCommand(Guid resumeId)
        {
            ResumeId = resumeId;
        }
    }

    public class DeleteResumeCommandHandler : IRequestHandler<DeleteResumeCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteResumeCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(DeleteResumeCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(cp => cp.UserId == currentUserId.Value, cancellationToken);

            if (candidateProfile == null)
                throw new KeyNotFoundException("Candidate profile not found.");

            var resume = await _context.Resumes
                .FirstOrDefaultAsync(r =>
                    r.Id == request.ResumeId &&
                    r.CandidateProfileId == candidateProfile.Id &&
                    r.DeletedAtUtc == null,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            if (resume.IsPrimary)
            {
                var nextPrimaryResume = await _context.Resumes
                    .Where(r =>
                        r.CandidateProfileId == candidateProfile.Id &&
                        r.Id != resume.Id &&
                        r.DeletedAtUtc == null)
                    .OrderByDescending(r => r.UpdatedAtUtc)
                    .FirstOrDefaultAsync(cancellationToken);

                if (nextPrimaryResume != null)
                {
                    nextPrimaryResume.IsPrimary = true;
                    nextPrimaryResume.UpdatedAtUtc = DateTime.UtcNow;
                }
            }

            resume.DeletedAtUtc = DateTime.UtcNow;
            resume.UpdatedAtUtc = DateTime.UtcNow;
            resume.IsPrimary = false;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}