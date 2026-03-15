using HiredIn.Backend.Domain.Common;
using HiredIn.Backend.Domain.Enums;

namespace HiredIn.Backend.Domain.Entities
{
    public class Vacancy : BaseEntity
    {
        public Guid CompanyId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public EmploymentType EmploymentType { get; set; }
        public WorkFormat WorkFormat { get; set; }
        public string? City { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public ExperienceLevel ExperienceLevel { get; set; }
        public VacancyStatus Status { get; set; }

        public Company Company { get; set; } = null!;
        public ICollection<VacancySkill> VacancySkills { get; set; } = new List<VacancySkill>();
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<FavouriteVacancy> FavouriteVacancies { get; set; } = new List<FavouriteVacancy>();

    }
}