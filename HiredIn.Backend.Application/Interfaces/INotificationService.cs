using HiredIn.Backend.Domain.Enums;

namespace HiredIn.Backend.Application.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(
            Guid userId,
            NotificationType type,
            string title,
            string text,
            CancellationToken cancellationToken = default);
    }
}