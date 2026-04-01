using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeSkill
{
    public class DeleteResumeSkillCommand : IRequest
    {
        public Guid ResumeId { get; set; }
        public Guid SkillId { get; set; }

        public DeleteResumeSkillCommand(Guid resumeId, Guid skillId)
        {
            ResumeId = resumeId;
            SkillId = skillId;
        }
    }

    public class DeleteResumeSkillCommandHandler : IRequestHandler<DeleteResumeSkillCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteResumeSkillCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(DeleteResumeSkillCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var resume = await _context.Resumes
                .Include(r => r.CandidateProfile)
                .FirstOrDefaultAsync(r =>
                    r.Id == request.ResumeId &&
                    r.DeletedAtUtc == null,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            if (resume.CandidateProfile.UserId != currentUserId.Value)
                throw new UnauthorizedAccessException("You do not have access to this resume.");

            var resumeSkill = await _context.ResumeSkills
                .FirstOrDefaultAsync(rs =>
                    rs.ResumeId == request.ResumeId &&
                    rs.SkillId == request.SkillId,
                    cancellationToken);

            if (resumeSkill == null)
                throw new KeyNotFoundException("Resume skill not found.");

            _context.ResumeSkills.Remove(resumeSkill);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}