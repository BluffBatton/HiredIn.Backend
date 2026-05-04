using Microsoft.EntityFrameworkCore;
using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Application.Interfaces;

namespace HiredIn.Backend.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<CandidateProfile> CandidateProfiles { get; set; } = null!;
        public DbSet<Resume> Resumes { get; set; } = null!;
        public DbSet<ResumeEducation> ResumeEducations { get; set; } = null!;
        public DbSet<ResumeFile> ResumeFiles { get; set; } = null!;
        public DbSet<ResumeWorkExperience> ResumeWorkExperiences { get; set; } = null!;
        public DbSet<ResumeSkill> ResumeSkills { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<CompanyMember> CompanyMembers { get; set; } = null!;
        public DbSet<CompanyRating> CompanyRatings { get; set; } = null!;
        public DbSet<Vacancy> Vacancies { get; set; } = null!;
        public DbSet<VacancySkill> VacancySkills { get; set; } = null!;
        public DbSet<Domain.Entities.Application> Applications { get; set; } = null!;
        public DbSet<Chat> Chats { get; set; } = null!;
        public DbSet<ChatParticipant> ChatParticipants { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<FavouriteVacancy> FavouriteVacancies { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
