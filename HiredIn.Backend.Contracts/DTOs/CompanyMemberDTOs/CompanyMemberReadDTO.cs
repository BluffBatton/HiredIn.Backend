using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.CompanyMemberDTOs
{
    public class CompanyMemberReadDTO
    {
        public Guid Id { get; set; }
        public required string FullName { get; set; }
        public required string CompanyName { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public CompanyMemberRole Role { get; set; }
    }
}
