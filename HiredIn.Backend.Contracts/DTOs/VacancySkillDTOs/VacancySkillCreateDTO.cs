namespace HiredIn.Backend.Contracts.DTOs.VacancySkillDTOs
{
    public class VacancySkillCreateDTO
    {
        public Guid VacancyId { get; set; }
        public Guid SkillId { get; set; }
        public bool IsRequired { get; set; }
    }
}