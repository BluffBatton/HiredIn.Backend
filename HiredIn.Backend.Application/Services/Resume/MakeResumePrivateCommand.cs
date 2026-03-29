using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Resume
{
    public class MakeResumePrivateCommand : IRequest
    {
        public Guid ResumeId { get; set; }

        public MakeResumePrivateCommand(Guid resumeId)
        {
            ResumeId = resumeId;
        }
    }

    public class MakeResumePrivateCommandHandler : IRequestHandler<MakeResumePrivateCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public MakeResumePrivateCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(MakeResumePrivateCommand request, CancellationToken cancellationToken)
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

            resume.Visibility = ResumeVisibility.Private;
            resume.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}