using HiredIn.Backend.Domain.Common;

namespace HiredIn.Backend.Domain.Entities
{
    public class Skill : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<ResumeSkill> ResumeSkills { get; set; } = new List<ResumeSkill>();
        public ICollection<VacancySkill> VacancySkills { get; set; } = new List<VacancySkill>();

    }
}