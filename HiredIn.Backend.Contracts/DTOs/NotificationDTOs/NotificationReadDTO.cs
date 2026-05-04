using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.NotificationDTOs
{
    public class NotificationReadDTO
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public NotificationType Type { get; set; }

        public string Title { get; set; } = null!;

        public string Text { get; set; } = null!;

        public bool IsRead { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}