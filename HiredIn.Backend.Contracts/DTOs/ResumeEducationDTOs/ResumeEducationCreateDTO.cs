namespace HiredIn.Backend.Contracts.DTOs.ResumeEducationDTOs
{
    public class ResumeEducationCreateDTO
    {
        public Guid ResumeId { get; set; }
        public string InstitutionName { get; set; } = null!;
        public string Degree { get; set; } = null!;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }
}
