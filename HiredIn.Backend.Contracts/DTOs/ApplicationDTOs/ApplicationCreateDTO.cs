using HiredIn.Backend.Contracts.DTOs.Enums;

namespace HiredIn.Backend.Contracts.DTOs.ApplicationDTOs
{
    public class ApplicationCreateDTO
    {
        public Guid VacancyId { get; set; }
        public Guid ResumeId { get; set; }
        public string? CoverLetter { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}
