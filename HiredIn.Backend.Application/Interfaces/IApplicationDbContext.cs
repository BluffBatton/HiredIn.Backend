using Microsoft.EntityFrameworkCore;
using HiredIn.Backend.Domain.Entities;

namespace HiredIn.Backend.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<CandidateProfile> CandidateProfiles { get; }
        DbSet<Resume> Resumes { get; }
        DbSet<ResumeEducation> ResumeEducations { get; }
        DbSet<ResumeFile> ResumeFiles { get; }
        DbSet<ResumeWorkExperience> ResumeWorkExperiences { get; }
        DbSet<ResumeSkill> ResumeSkills { get; }
        DbSet<Skill> Skills { get; }
        DbSet<Company> Companies { get; }
        DbSet<CompanyMember> CompanyMembers { get; }
        DbSet<Vacancy> Vacancies { get; }
        DbSet<VacancySkill> VacancySkills { get; }
        DbSet<Domain.Entities.Application> Applications { get; }
        DbSet<Chat> Chats { get; }
        DbSet<ChatParticipant> ChatParticipants { get; }
        DbSet<Message> Messages { get; }
        DbSet<Notification> Notifications { get; }
        DbSet<FavouriteVacancy> FavouriteVacancies { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
