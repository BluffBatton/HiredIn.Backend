using HiredIn.Backend.Domain.Common;
using HiredIn.Backend.Domain.Enums;

namespace HiredIn.Backend.Domain.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Industry { get; set; }
        public string? City { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public CompanyStatus Status { get; set; }

        public ICollection<CompanyMember> Members { get; set; } = new List<CompanyMember>();
        public ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
    }
}