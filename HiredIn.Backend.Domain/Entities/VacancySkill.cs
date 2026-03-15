namespace HiredIn.Backend.Domain.Entities
{
    public class VacancySkill
    {
        public Guid VacancyId { get; set; }
        public Guid SkillId { get; set; }
        public bool IsRequired { get; set; }

        public Vacancy Vacancy { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}
