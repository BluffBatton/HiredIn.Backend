using HiredIn.Backend.Domain.Common;
using HiredIn.Backend.Domain.Enums;

namespace HiredIn.Backend.Domain.Entities
{
    public class CompanyMember : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public CompanyMemberRole Role { get; set; }

        public Company Company { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}