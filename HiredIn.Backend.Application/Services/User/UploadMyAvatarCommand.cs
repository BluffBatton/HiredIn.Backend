using HiredIn.Backend.Application.Common.Files;
using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.User
{
    public class UploadMyAvatarCommand : IRequest<string>
    {
        public string ContentType { get; set; }
        public long Size { get; set; }
        public byte[] FileBytes { get; set; }

        public UploadMyAvatarCommand(string contentType, long size, byte[] fileBytes)
        {
            ContentType = contentType;
            Size = size;
            FileBytes = fileBytes;
        }
    }

    public class UploadMyAvatarCommandHandler : IRequestHandler<UploadMyAvatarCommand, string>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IFileStorageService _fileStorageService;

        public UploadMyAvatarCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IFileStorageService fileStorageService)
        {
            _context = context;
            _userContextService = userContextService;
            _fileStorageService = fileStorageService;
        }

        public async Task<string> Handle(UploadMyAvatarCommand request, CancellationToken cancellationToken)
        {
            FileValidationHelper.ValidateImage(request.ContentType, request.Size);

            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId.Value && u.DeletedAtUtc == null, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var path = $"users/{userId.Value}/avatar";

            await _fileStorageService.UploadAsync(
                "avatars",
                path,
                request.FileBytes,
                cancellationToken);

            var publicUrl = _fileStorageService.GetPublicUrl("avatars", path);

            user.AvatarUrl = publicUrl;
            user.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return publicUrl;
        }
    }
}