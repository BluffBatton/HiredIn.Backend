using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.ResumeSkillDTOs
{
    public class ResumeSkillCreateDTO
    {
        public Guid ResumeId { get; set; }
        public Guid SkillId { get; set; }
        public SkillLevel Level { get; set; }
    }
}