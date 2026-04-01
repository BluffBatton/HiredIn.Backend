namespace HiredIn.Backend.Contracts.DTOs.ResumeWorkExperienceDTOs
{
    public class ResumeWorkExperienceReadDTO
    {
        public Guid ResumeId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string PositionTitle { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Description { get; set; }
    }
}