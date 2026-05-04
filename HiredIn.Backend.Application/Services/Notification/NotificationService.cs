using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Domain.Enums;

namespace HiredIn.Backend.Application.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IApplicationDbContext _context;

        public NotificationService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(
            Guid userId,
            NotificationType type,
            string title,
            string text,
            CancellationToken cancellationToken = default)
        {
            var notification = new Domain.Entities.Notification
            {
                UserId = userId,
                Type = type,
                Title = title,
                Text = text,
                IsRead = false,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _context.Notifications.AddAsync(notification, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}