using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
//using DomainResume = HiredIn.Backend.Domain.Entities.Resume;

namespace HiredIn.Backend.Application.Services.Resume
{
    public class CreateResumeCommand : IRequest
    {
        public ResumeCreateDTO Resume { get; set; }

        public CreateResumeCommand(ResumeCreateDTO resume)
        {
            Resume = resume;
        }
    }

    public class CreateResumeCommandHandler : IRequestHandler<CreateResumeCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public CreateResumeCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task Handle(CreateResumeCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var candidateProfile = await _context.CandidateProfiles
                .FirstOrDefaultAsync(cp => cp.UserId == currentUserId.Value, cancellationToken);

            if (candidateProfile == null)
                throw new KeyNotFoundException("Candidate profile not found.");

            var existingResumes = await _context.Resumes
                .Where(r => r.CandidateProfileId == candidateProfile.Id)
                .ToListAsync(cancellationToken);

            if (request.Resume.IsPrimary)
            {
                foreach (var existingResume in existingResumes)
                {
                    existingResume.IsPrimary = false;
                    existingResume.UpdatedAtUtc = DateTime.UtcNow;
                }
            }

            var resume = _mapper.Map<Domain.Entities.Resume>(request.Resume);
            resume.CandidateProfileId = candidateProfile.Id;
            resume.CreatedAtUtc = DateTime.UtcNow;
            resume.UpdatedAtUtc = DateTime.UtcNow;

            if (!existingResumes.Any())
                resume.IsPrimary = true;

            await _context.Resumes.AddAsync(resume, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return;
        }
    }
}