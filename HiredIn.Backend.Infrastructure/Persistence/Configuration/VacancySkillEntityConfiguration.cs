using HiredIn.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class VacancySkillEntityConfiguration : IEntityTypeConfiguration<VacancySkill>
    {
        public void Configure(EntityTypeBuilder<VacancySkill> builder)
        {
            builder.ToTable("VacancySkills");

            builder.HasKey(vs => new { vs.VacancyId, vs.SkillId });

            builder.Property(vs => vs.IsRequired)
                   .IsRequired();

            builder.HasOne(vs => vs.Vacancy)
                   .WithMany(v => v.VacancySkills)
                   .HasForeignKey(vs => vs.VacancyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(vs => vs.Skill)
                   .WithMany(s => s.VacancySkills)
                   .HasForeignKey(vs => vs.SkillId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}