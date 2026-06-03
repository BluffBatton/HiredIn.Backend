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

        public Guid CandidateProfileId { get; set; }
        public string CandidateName { get; set; } = null!;
        public string? CandidateCity { get; set; }
        public string? CandidateAbout { get; set; }
        public bool CandidateOpenToWork { get; set; }

        public string DesiredPosition { get; set; } = null!;
        public EmploymentType EmploymentType { get; set; }
        public WorkFormat WorkFormat { get; set; }
        public ExperienceLevel ExperienceLevel { get; set; }
    }
}
