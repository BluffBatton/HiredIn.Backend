namespace HiredIn.Backend.Contracts.MessagesDTOs
{
    public class MessageReadDTO
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid SenderUserId { get; set; }

        public string SenderFullName { get; set; } = null!;
        public string Content { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; }
    }
}
