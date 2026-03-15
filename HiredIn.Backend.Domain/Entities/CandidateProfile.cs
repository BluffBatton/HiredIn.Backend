using HiredIn.Backend.Domain.Common;

namespace HiredIn.Backend.Domain.Entities
{
    public class CandidateProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? City { get; set; }
        public string? About { get; set; }
        public bool OpenToWork { get; set; }

        public User User { get; set; } = null!;
        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    }
}