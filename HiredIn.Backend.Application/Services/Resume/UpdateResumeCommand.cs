using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Resume
{
    public class PatchResumeCommand : IRequest
    {
        public Guid ResumeId { get; set; }
        public ResumeUpdateDTO Resume { get; set; }

        public PatchResumeCommand(Guid resumeId, ResumeUpdateDTO resume)
        {
            ResumeId = resumeId;
            Resume = resume;
        }
    }

    public class PatchResumeCommandHandler : IRequestHandler<PatchResumeCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public PatchResumeCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(PatchResumeCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(cp => cp.UserId == currentUserId.Value, cancellationToken);

            if (candidateProfile == null)
                throw new KeyNotFoundException("Candidate profile not found.");

            var resume = await _context.Resumes
                .FirstOrDefaultAsync(
                    r => r.Id == request.ResumeId &&
                         r.CandidateProfileId == candidateProfile.Id,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            var dto = request.Resume;

            if (dto.Title != null)
                resume.Title = dto.Title;

            if (dto.DesiredPosition != null)
                resume.DesiredPosition = dto.DesiredPosition;

            if (dto.Summary != null)
                resume.Summary = dto.Summary;

            if (dto.EmploymentType.HasValue)
                resume.EmploymentType = (HiredIn.Backend.Domain.Enums.EmploymentType)dto.EmploymentType.Value;

            if (dto.WorkFormat.HasValue)
                resume.WorkFormat = (HiredIn.Backend.Domain.Enums.WorkFormat)dto.WorkFormat.Value;

            if (dto.ExperienceLevel.HasValue)
                resume.ExperienceLevel = (HiredIn.Backend.Domain.Enums.ExperienceLevel)dto.ExperienceLevel.Value;

            if (dto.Visibility.HasValue)
                resume.Visibility = (HiredIn.Backend.Domain.Enums.ResumeVisibility)dto.Visibility.Value;

            if (dto.IsPrimary.HasValue)
            {
                if (dto.IsPrimary.Value)
                {
                    var otherResumes = await _context.Resumes
                        .Where(r => r.CandidateProfileId == candidateProfile.Id && r.Id != resume.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var otherResume in otherResumes)
                    {
                        otherResume.IsPrimary = false;
                        otherResume.UpdatedAtUtc = DateTime.UtcNow;
                    }
                }

                resume.IsPrimary = dto.IsPrimary.Value;
            }

            resume.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}