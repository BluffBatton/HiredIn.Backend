using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Notification
{
    public class DeleteNotificationCommand : IRequest
    {
        public Guid NotificationId { get; set; }

        public DeleteNotificationCommand(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }

    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public DeleteNotificationCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(
            DeleteNotificationCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.Id == request.NotificationId &&
                    n.UserId == userId.Value &&
                    n.DeletedAtUtc == null,
                    cancellationToken);

            if (notification == null)
                throw new KeyNotFoundException("Notification not found.");

            notification.DeletedAtUtc = DateTime.UtcNow;
            notification.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}