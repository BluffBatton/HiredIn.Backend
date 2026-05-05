using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeFile
{
    public class GetResumeFileUrlQuery : IRequest<string>
    {
        public Guid ResumeId { get; set; }

        public GetResumeFileUrlQuery(Guid resumeId)
        {
            ResumeId = resumeId;
        }
    }

    public class GetResumeFileUrlQueryHandler : IRequestHandler<GetResumeFileUrlQuery, string>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IFileStorageService _fileStorageService;

        public GetResumeFileUrlQueryHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IFileStorageService fileStorageService)
        {
            _context = context;
            _userContextService = userContextService;
            _fileStorageService = fileStorageService;
        }

        public async Task<string> Handle(GetResumeFileUrlQuery request, CancellationToken cancellationToken)
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

            var isOwner = resume.CandidateProfile.UserId == currentUserId.Value;

            var resumeFile = await _context.ResumeFiles
                .FirstOrDefaultAsync(rf =>
                    rf.ResumeId == request.ResumeId &&
                    rf.DeletedAtUtc == null,
                    cancellationToken);

            if (resumeFile == null)
                throw new KeyNotFoundException("Resume file not found.");

            if (!isOwner)
            {
                var hasApplicationAccess = await _context.Applications
                    .AnyAsync(a =>
                        a.ResumeId == request.ResumeId &&
                        a.DeletedAtUtc == null &&
                        _context.CompanyMembers.Any(cm =>
                            cm.CompanyId == a.Vacancy.CompanyId &&
                            cm.UserId == currentUserId.Value),
                        cancellationToken);

                if (!hasApplicationAccess)
                    throw new UnauthorizedAccessException("You do not have access to this resume file.");
            }

            var signedUrl = await _fileStorageService.CreateSignedUrlAsync(
                "resume-files",
                resumeFile.FileUrl,
                300,
                cancellationToken);

            return signedUrl;
        }
    }
}