using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeWorkExperienceDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainResumeWorkExperience = HiredIn.Backend.Domain.Entities.ResumeWorkExperience;

namespace HiredIn.Backend.Application.Services.ResumeWorkExperience
{
    public class CreateResumeWorkExperienceCommand : IRequest<ResumeWorkExperienceReadDTO>
    {
        public ResumeWorkExperienceCreateDTO WorkExperience { get; set; }

        public CreateResumeWorkExperienceCommand(ResumeWorkExperienceCreateDTO workExperience)
        {
            WorkExperience = workExperience;
        }
    }

    public class CreateResumeWorkExperienceCommandHandler : IRequestHandler<CreateResumeWorkExperienceCommand, ResumeWorkExperienceReadDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public CreateResumeWorkExperienceCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<ResumeWorkExperienceReadDTO> Handle(CreateResumeWorkExperienceCommand request, CancellationToken cancellationToken)
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
                    r.Id == request.WorkExperience.ResumeId &&
                    r.CandidateProfileId == candidateProfile.Id &&
                    r.DeletedAtUtc == null,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            var alreadyExists = await _context.ResumeWorkExperiences
                .AnyAsync(w =>
                    w.ResumeId == request.WorkExperience.ResumeId &&
                    w.CompanyName == request.WorkExperience.CompanyName &&
                    w.PositionTitle == request.WorkExperience.PositionTitle,
                    cancellationToken);

            if (alreadyExists)
                throw new InvalidOperationException("This work experience already exists.");

            var workExperience = _mapper.Map<DomainResumeWorkExperience>(request.WorkExperience);
            workExperience.Resume = resume;

            await _context.ResumeWorkExperiences.AddAsync(workExperience, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<ResumeWorkExperienceReadDTO>(workExperience);
        }
    }
}