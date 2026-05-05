using HiredIn.Backend.Application.Common.Files;
using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.ResumeFile
{
    public class UploadResumeFileCommand : IRequest<Guid>
    {
        public Guid ResumeId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public byte[] FileBytes { get; set; }
        public string Extension { get; set; }

        public UploadResumeFileCommand(
            Guid resumeId,
            string fileName,
            string contentType,
            long size,
            byte[] fileBytes,
            string extension)
        {
            ResumeId = resumeId;
            FileName = fileName;
            ContentType = contentType;
            Size = size;
            FileBytes = fileBytes;
            Extension = extension;
        }
    }

    public class UploadResumeFileCommandHandler : IRequestHandler<UploadResumeFileCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IFileStorageService _fileStorageService;

        public UploadResumeFileCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IFileStorageService fileStorageService)
        {
            _context = context;
            _userContextService = userContextService;
            _fileStorageService = fileStorageService;
        }

        public async Task<Guid> Handle(UploadResumeFileCommand request, CancellationToken cancellationToken)
        {
            FileValidationHelper.ValidateResumeFile(request.ContentType, request.Size);

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
                throw new UnauthorizedAccessException("You can upload file only for your own resume.");

            var existingFile = await _context.ResumeFiles
                .FirstOrDefaultAsync(rf =>
                    rf.ResumeId == request.ResumeId &&
                    rf.DeletedAtUtc == null,
                    cancellationToken);

            if (existingFile != null)
            {
                await _fileStorageService.DeleteAsync(
                    "resume-files",
                    existingFile.FileUrl,
                    cancellationToken);

                existingFile.DeletedAtUtc = DateTime.UtcNow;
                existingFile.UpdatedAtUtc = DateTime.UtcNow;
            }

            var path = $"resumes/{request.ResumeId}/resume{request.Extension}";

            await _fileStorageService.UploadAsync(
                "resume-files",
                path,
                request.FileBytes,
                cancellationToken);

            var resumeFile = new Domain.Entities.ResumeFile
            {
                ResumeId = request.ResumeId,
                FileName = request.FileName,
                FileUrl = path,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _context.ResumeFiles.AddAsync(resumeFile, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return resumeFile.Id;
        }
    }
}