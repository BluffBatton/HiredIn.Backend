using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Admin
{
    public class DeleteAdminUserCommand : IRequest
    {
        public Guid UserId { get; set; }

        public DeleteAdminUserCommand(Guid userId)
        {
            UserId = userId;
        }
    }

    public class DeleteAdminUserCommandHandler : IRequestHandler<DeleteAdminUserCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteAdminUserCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(DeleteAdminUserCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _userContextService.GetCurrentUserId();

            if (currentUserId == request.UserId)
                throw new InvalidOperationException("Admin cannot delete own account.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Id == request.UserId &&
                    u.DeletedAtUtc == null,
                    cancellationToken);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.Status = UserStatus.Deleted;
            user.DeletedAtUtc = DateTime.UtcNow;
            user.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
