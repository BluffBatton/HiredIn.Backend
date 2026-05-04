using HiredIn.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Application.Services.Notification
{
    public class MarkNotificationAsReadCommand : IRequest
    {
        public Guid NotificationId { get; set; }

        public MarkNotificationAsReadCommand(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }

    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;

        public MarkNotificationAsReadCommandHandler(
            IApplicationDbContext context,
            IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(
            MarkNotificationAsReadCommand request,
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

            notification.IsRead = true;
            notification.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}