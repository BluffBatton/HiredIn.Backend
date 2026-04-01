using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeFileDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeFile
{
    public class CreateResumeFileCommand : IRequest<Guid>
    {
        public ResumeFileCreateDTO ResumeFile { get; set; }

        public CreateResumeFileCommand(ResumeFileCreateDTO resumeFile)
        {
            ResumeFile = resumeFile;
        }
    }

    public class CreateResumeFileCommandHandler : IRequestHandler<CreateResumeFileCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public CreateResumeFileCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<Guid> Handle(CreateResumeFileCommand request, CancellationToken cancellationToken)
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
                    r.Id == request.ResumeFile.ResumeId &&
                    r.CandidateProfileId == candidateProfile.Id &&
                    r.DeletedAtUtc == null,
                    cancellationToken);

            if (resume == null)
                throw new KeyNotFoundException("Resume not found.");

            var existingFile = await _context.ResumeFiles
                .FirstOrDefaultAsync(rf =>
                    rf.ResumeId == request.ResumeFile.ResumeId &&
                    rf.DeletedAtUtc == null,
                    cancellationToken);

            if (existingFile != null)
                throw new InvalidOperationException("Resume file already exists. Use PATCH to replace it.");

            var resumeFile = _mapper.Map<Domain.Entities.ResumeFile>(request.ResumeFile);
            resumeFile.CreatedAtUtc = DateTime.UtcNow;
            resumeFile.UpdatedAtUtc = DateTime.UtcNow;

            await _context.ResumeFiles.AddAsync(resumeFile, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return resumeFile.Id;
        }
    }
}