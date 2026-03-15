using HiredIn.Backend.Domain.Common;
using HiredIn.Backend.Domain.Enums;

namespace HiredIn.Backend.Domain.Entities
{
    public class Application : BaseEntity
    {
        public Guid VacancyId { get; set; }
        public Guid ResumeId { get; set; }
        public string? CoverLetter { get; set; }
        public ApplicationStatus Status { get; set; }

        public Vacancy Vacancy { get; set; } = null!;
        public Resume Resume { get; set; } = null!;
        public Chat? Chat { get; set; }
    }
}