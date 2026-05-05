namespace HiredIn.Backend.Contracts.DTOs.ChatDTOs
{
    public class ChatParticipantReadDTO
    {
        public Guid UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string? AvatarUrl { get; set; }
    }
}