using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeWorkExperienceDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeWorkExperience
{
    public class DeleteResumeWorkExperienceCommand : IRequest
    {
        public ResumeWorkExperienceDeleteDTO WorkExperience { get; set; }

        public DeleteResumeWorkExperienceCommand(ResumeWorkExperienceDeleteDTO workExperience)
        {
            WorkExperience = workExperience;
        }
    }

    public class DeleteResumeWorkExperienceCommandHandler : IRequestHandler<DeleteResumeWorkExperienceCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteResumeWorkExperienceCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(DeleteResumeWorkExperienceCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var resume = await _context.Resumes
                .Include(r => r.CandidateProfile)
                .FirstOrDefaultAsync(r =>
                    r.Id == request.WorkExperience.ResumeId &&
                    r.DeletedAtUtc == null,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            if (resume.CandidateProfile.UserId != currentUserId.Value)
                throw new UnauthorizedAccessException("You do not have access to this resume.");

            var workExperience = await _context.ResumeWorkExperiences
                .FirstOrDefaultAsync(w =>
                    w.ResumeId == request.WorkExperience.ResumeId &&
                    w.CompanyName == request.WorkExperience.CompanyName &&
                    w.PositionTitle == request.WorkExperience.PositionTitle,
                    cancellationToken);

            if (workExperience == null)
                throw new KeyNotFoundException("Resume work experience not found.");

            _context.ResumeWorkExperiences.Remove(workExperience);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}