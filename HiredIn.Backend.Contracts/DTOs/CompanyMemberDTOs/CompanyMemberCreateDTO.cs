using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.CompanyMemberDTOs
{
    public class CompanyMemberCreateDTO
    {
        //public Guid UserId { get; set; }
        public required string Email { get; set; }
        public CompanyMemberRole Role { get; set; }
    }
}
