using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.CandidateProfileDTOs
{
    public class CandidateSearchResultDTO
    {
        public Guid CandidateProfileId { get; set; }
        public Guid UserId { get; set; }
        public required string FullName { get; set; }
        public string? City { get; set; }
        public string? About { get; set; }
        public bool OpenToWork { get; set; }

        public Guid? PrimaryResumeId { get; set; }
        public string? PrimaryResumeTitle { get; set; }
        public string? DesiredPosition { get; set; }
        public string? Summary { get; set; }
        public EmploymentType? EmploymentType { get; set; }
        public WorkFormat? WorkFormat { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public List<CandidateSearchSkillDTO> Skills { get; set; } = new();
    }

    public class CandidateSearchSkillDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public SkillLevel Level { get; set; }
    }
}
