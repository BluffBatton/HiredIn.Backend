using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.User
{
    public class DeleteMyAvatarCommand : IRequest
    {
    }

    public class DeleteMyAvatarCommandHandler : IRequestHandler<DeleteMyAvatarCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IFileStorageService _fileStorageService;

        public DeleteMyAvatarCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService,
            IFileStorageService fileStorageService)
        {
            _context = context;
            _userContextService = userContextService;
            _fileStorageService = fileStorageService;
        }

        public async Task Handle(DeleteMyAvatarCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId.Value && u.DeletedAtUtc == null, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var path = $"users/{userId.Value}/avatar";

            await _fileStorageService.DeleteAsync("avatars", path, cancellationToken);

            user.AvatarUrl = null;
            user.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}