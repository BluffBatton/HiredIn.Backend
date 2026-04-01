using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeWorkExperienceDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainResumeWorkExperience = HiredIn.Backend.Domain.Entities.ResumeWorkExperience;

namespace HiredIn.Backend.Application.Services.ResumeWorkExperience
{
    public class PatchResumeWorkExperienceCommand : IRequest
    {
        public ResumeWorkExperiencePatchDTO WorkExperience { get; set; }

        public PatchResumeWorkExperienceCommand(ResumeWorkExperiencePatchDTO workExperience)
        {
            WorkExperience = workExperience;
        }
    }

    public class PatchResumeWorkExperienceCommandHandler : IRequestHandler<PatchResumeWorkExperienceCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public PatchResumeWorkExperienceCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(PatchResumeWorkExperienceCommand request, CancellationToken cancellationToken)
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

            if (request.WorkExperience.StartDate.HasValue)
                workExperience.StartDate = request.WorkExperience.StartDate;

            if (request.WorkExperience.EndDate.HasValue)
                workExperience.EndDate = request.WorkExperience.EndDate;

            if (request.WorkExperience.IsCurrent.HasValue)
                workExperience.IsCurrent = request.WorkExperience.IsCurrent.Value;

            if (request.WorkExperience.Description != null)
                workExperience.Description = request.WorkExperience.Description;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}