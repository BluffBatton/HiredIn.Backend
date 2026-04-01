namespace HiredIn.Backend.Contracts.DTOs.ResumeWorkExperienceDTOs
{
    public class ResumeWorkExperienceDeleteDTO
    {
        public Guid ResumeId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string PositionTitle { get; set; } = null!;
    }
}