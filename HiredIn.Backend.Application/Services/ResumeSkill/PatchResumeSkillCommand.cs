using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeSkillDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeSkill
{
    public class PatchResumeSkillCommand : IRequest
    {
        public Guid ResumeId { get; set; }
        public Guid SkillId { get; set; }
        public ResumeSkillPatchDTO ResumeSkill { get; set; }

        public PatchResumeSkillCommand(Guid resumeId, Guid skillId, ResumeSkillPatchDTO resumeSkill)
        {
            ResumeId = resumeId;
            SkillId = skillId;
            ResumeSkill = resumeSkill;
        }
    }

    public class PatchResumeSkillCommandHandler : IRequestHandler<PatchResumeSkillCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public PatchResumeSkillCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(PatchResumeSkillCommand request, CancellationToken cancellationToken)
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

            if (request.ResumeSkill.Level.HasValue)
                resumeSkill.Level = (HiredIn.Backend.Domain.Enums.SkillLevel)request.ResumeSkill.Level.Value;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}