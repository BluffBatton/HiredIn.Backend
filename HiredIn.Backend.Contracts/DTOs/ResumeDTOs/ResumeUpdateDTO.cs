using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.ResumeDTOs
{
    public class ResumeUpdateDTO
    {
        public string? Title { get; set; }
        public string? DesiredPosition { get; set; }
        public string? Summary { get; set; }
        public EmploymentType? EmploymentType { get; set; }
        public WorkFormat? WorkFormat { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }
        public ResumeVisibility? Visibility { get; set; }
        public bool? IsPrimary { get; set; }
    }
}