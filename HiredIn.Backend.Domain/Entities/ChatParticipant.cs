namespace HiredIn.Backend.Domain.Entities
{
    public class ChatParticipant
    {
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }

        public Chat Chat { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
