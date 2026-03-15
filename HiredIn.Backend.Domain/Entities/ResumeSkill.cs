using HiredIn.Backend.Domain.Enums;

namespace HiredIn.Backend.Domain.Entities
{
    public class ResumeSkill
    {
        public Guid ResumeId { get; set; }
        public Guid SkillId { get; set; }
        public SkillLevel Level { get; set; }

        public Resume Resume { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}