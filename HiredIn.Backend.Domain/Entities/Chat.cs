using HiredIn.Backend.Domain.Common;
using HiredIn.Backend.Domain.Enums;

namespace HiredIn.Backend.Domain.Entities
{
    public class Chat : BaseEntity
    {
        public Guid ApplicationId { get; set; }
        public ChatStatus Status { get; set; }

        public Application Application { get; set; } = null!;
        public ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}