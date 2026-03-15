using HiredIn.Backend.Domain.Common;

namespace HiredIn.Backend.Domain.Entities
{
    public class Message : BaseEntity
    {
        public Guid ChatId { get; set; }
        public Guid SenderUserId { get; set; }
        public string Text { get; set; } = null!;
        public bool IsRead { get; set; }

        public Chat Chat { get; set; } = null!;
        public User SenderUser { get; set; } = null!;
    }
}