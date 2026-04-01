using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeFile
{
    public class DeleteResumeFileCommand : IRequest
    {
        public Guid Id { get; set; }

        public DeleteResumeFileCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteResumeFileCommandHandler : IRequestHandler<DeleteResumeFileCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteResumeFileCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(DeleteResumeFileCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var resumeFile = await _context.ResumeFiles
                .Include(rf => rf.Resume)
                .ThenInclude(r => r.CandidateProfile)
                .FirstOrDefaultAsync(rf =>
                    rf.Id == request.Id &&
                    rf.DeletedAtUtc == null,
                    cancellationToken);

            if (resumeFile == null)
                throw new KeyNotFoundException("Resume file not found.");

            if (resumeFile.Resume.CandidateProfile.UserId != currentUserId.Value)
                throw new UnauthorizedAccessException("You do not have access to this resume file.");

            resumeFile.DeletedAtUtc = DateTime.UtcNow;
            resumeFile.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}