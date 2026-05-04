using HiredIn.Backend.Domain.Common;

namespace HiredIn.Backend.Domain.Entities
{
    public class CompanyRating : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public Company Company { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
