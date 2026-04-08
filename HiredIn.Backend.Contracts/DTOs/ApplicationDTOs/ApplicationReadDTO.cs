using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.ApplicationDTOs
{
    public class ApplicationReadDTO
    {
        public Guid Id { get; set; }
        public Guid VacancyId { get; set; }
        public Guid ResumeId { get; set; }

        public string? CoverLetter { get; set; }
        public ApplicationStatus Status { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public string VacancyTitle { get; set; } = null!;
        public string ResumeTitle { get; set; } = null!;
        public string CompanyName { get; set; } = null!;

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? City { get; set; }
    }
}
