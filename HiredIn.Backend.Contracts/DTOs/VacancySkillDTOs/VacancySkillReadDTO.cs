namespace HiredIn.Backend.Contracts.DTOs.VacancySkillDTOs
{
    public class VacancySkillReadDTO
    {
        public Guid VacancyId { get; set; }
        public Guid SkillId { get; set; }
        public string SkillName { get; set; } = null!;
        public bool IsRequired { get; set; }
    }
}