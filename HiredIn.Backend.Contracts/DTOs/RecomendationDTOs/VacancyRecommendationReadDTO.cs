namespace HiredIn.Backend.Contracts.DTOs.RecommendationDTOs
{
    public class VacancyRecommendationReadDTO
    {
        public Guid VacancyId { get; set; }

        public string Title { get; set; } = null!;
        public string CompanyName { get; set; } = null!;

        public string? City { get; set; }

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }

        public string EmploymentType { get; set; } = null!;
        public string WorkFormat { get; set; } = null!;
        public string ExperienceLevel { get; set; } = null!;

        public int MatchScore { get; set; }

        public List<string> MatchedSkills { get; set; } = new();
        public List<string> MissingSkills { get; set; } = new();
    }
}