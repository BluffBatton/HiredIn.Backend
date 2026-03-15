using HiredIn.Backend.Domain.Common;
using HiredIn.Backend.Domain.Enums;

namespace HiredIn.Backend.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = null!;
        public string Text { get; set; } = null!;
        public bool IsRead { get; set; }

        public User User { get; set; } = null!;
    }
}