using HiredIn.Backend.Domain.Common;
using HiredIn.Backend.Domain.Enums;

namespace HiredIn.Backend.Domain.Entities
{
    public class Resume : BaseEntity
    {
        public Guid CandidateProfileId { get; set; }
        public string Title { get; set; } = null!;
        public string DesiredPosition { get; set; } = null!;
        public string? Summary { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public WorkFormat WorkFormat { get; set; }
        public ExperienceLevel ExperienceLevel { get; set; }
        public ResumeVisibility Visibility { get; set; }
        public bool IsPrimary { get; set; }

        public CandidateProfile CandidateProfile { get; set; } = null!;
        public ResumeFile? ResumeFile { get; set; }
        public ICollection<ResumeEducation> Educations { get; set; } = new List<ResumeEducation>();
        public ICollection<ResumeWorkExperience> WorkExperiences { get; set; } = new List<ResumeWorkExperience>();
        public ICollection<ResumeSkill> ResumeSkills { get; set; } = new List<ResumeSkill>();
        public ICollection<Application> Applications { get; set; } = new List<Application>();

    }
}
