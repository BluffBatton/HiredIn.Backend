using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeEducation
{
    public class DeleteResumeEducationCommand : IRequest
    {
        public Guid Id { get; set; }

        public DeleteResumeEducationCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteResumeEducationCommandHandler : IRequestHandler<DeleteResumeEducationCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteResumeEducationCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(DeleteResumeEducationCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var education = await _context.ResumeEducations
                .Include(e => e.Resume)
                .ThenInclude(r => r.CandidateProfile)
                .FirstOrDefaultAsync(e =>
                    e.Id == request.Id &&
                    e.DeletedAtUtc == null,
                    cancellationToken);

            if (education == null)
                throw new KeyNotFoundException("Resume education not found.");

            if (education.Resume.CandidateProfile.UserId != currentUserId.Value)
                throw new UnauthorizedAccessException("You do not have access to this resume education.");

            education.DeletedAtUtc = DateTime.UtcNow;
            education.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}