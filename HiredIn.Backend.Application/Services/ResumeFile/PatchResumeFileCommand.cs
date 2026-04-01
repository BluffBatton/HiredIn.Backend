using AutoMapper;
using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Contracts.DTOs.ResumeFileDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeFile
{
    public class PatchResumeFileCommand : IRequest
    {
        public Guid Id { get; set; }
        public ResumeFilePatchDTO ResumeFile { get; set; }

        public PatchResumeFileCommand(Guid id, ResumeFilePatchDTO resumeFile)
        {
            Id = id;
            ResumeFile = resumeFile;
        }
    }

    public class PatchResumeFileCommandHandler : IRequestHandler<PatchResumeFileCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;

        public PatchResumeFileCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IUserContextService userContextService)
        {
            _context = context;
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task Handle(PatchResumeFileCommand request, CancellationToken cancellationToken)
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

            _mapper.Map(request.ResumeFile, resumeFile);
            resumeFile.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}