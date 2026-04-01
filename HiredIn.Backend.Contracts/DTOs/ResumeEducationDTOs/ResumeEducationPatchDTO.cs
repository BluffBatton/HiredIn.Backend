namespace HiredIn.Backend.Contracts.DTOs.ResumeEducationDTOs
{
    public class ResumeEducationPatchDTO
    {
        public string? InstitutionName { get; set; }
        public string? Degree { get; set; }
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }
}
