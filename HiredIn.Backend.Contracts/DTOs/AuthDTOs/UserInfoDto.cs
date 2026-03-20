using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.AuthDTOs
{
    public class UserInfoDto
    {
        public Guid ID { get; set; }
        public required string FullName { get; set; }
        public required UserRole Role { get; set; }
    }
}
