using HiredIn.Backend.Domain.Common;

namespace HiredIn.Backend.Domain.Entities
{
    public class ResumeEducation : BaseEntity
    {
        public Guid ResumeId { get; set; }
        public string InstitutionName { get; set; } = null!;
        public string Degree { get; set; } = null!;
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }

        public Resume Resume { get; set; } = null!;
    }
}