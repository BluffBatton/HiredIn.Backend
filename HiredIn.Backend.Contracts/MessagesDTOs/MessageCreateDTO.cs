namespace HiredIn.Backend.Contracts.MessagesDTOs
{
    public class MessageCreateDTO
    {
        public Guid ChatId { get; set; }
        public string Content { get; set; } = null!;
    }
}
