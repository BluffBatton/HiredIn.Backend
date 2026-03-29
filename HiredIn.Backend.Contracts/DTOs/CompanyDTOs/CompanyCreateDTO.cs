using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.CompanyDTOs
{
    public class CompanyCreateDTO
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Industry { get; set; }
        public string? City { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public CompanyStatus Status { get; set; }
    }
}
