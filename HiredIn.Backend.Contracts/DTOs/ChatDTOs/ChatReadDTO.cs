using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.ChatDTOs
{
    public class ChatReadDTO
    {
        public Guid Id { get; set; }

        public Guid ApplicationId { get; set; }

        public ChatStatus Status { get; set; }

        public string VacancyTitle { get; set; } = null!;

        public List<ChatParticipantReadDTO> Participants { get; set; } = new();

        public string? LastMessageText { get; set; }

        public DateTime? LastMessageCreatedAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}