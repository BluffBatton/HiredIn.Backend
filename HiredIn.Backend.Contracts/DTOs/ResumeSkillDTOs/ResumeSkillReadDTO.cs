using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.ResumeSkillDTOs
{
    public class ResumeSkillReadDTO
    {
        public Guid ResumeId { get; set; }
        public Guid SkillId { get; set; }
        public string SkillName { get; set; } = null!;
        public SkillLevel Level { get; set; }
    }
}