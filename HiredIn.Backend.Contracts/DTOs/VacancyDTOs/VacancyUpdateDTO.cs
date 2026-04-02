using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.VacancyDTOs
{
    public class VacancyUpdateDTO
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public EmploymentType EmploymentType { get; set; }
        public WorkFormat WorkFormat { get; set; }
        public string? City { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public ExperienceLevel ExperienceLevel { get; set; }
        public VacancyStatus Status { get; set; }
    }
}