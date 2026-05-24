using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.UserDTOs
{
    public class UserReadDTO
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set;  }
        public string? AvatarUrl { get; set; }
        public UserRole UserRole { get; set; }
        public UserStatus UserStatus { get; set; }
    }
}
