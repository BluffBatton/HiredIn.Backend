using HiredIn.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class ResumeSkillEntityConfiguration : IEntityTypeConfiguration<ResumeSkill>
    {
        public void Configure(EntityTypeBuilder<ResumeSkill> builder)
        {
            builder.ToTable("ResumeSkills");

            builder.HasKey(rs => new { rs.ResumeId, rs.SkillId });

            builder.Property(rs => rs.Level)
                   .HasConversion<string>()
                   .IsRequired();

            builder.HasOne(rs => rs.Resume)
                   .WithMany(r => r.ResumeSkills)
                   .HasForeignKey(rs => rs.ResumeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rs => rs.Skill)
                   .WithMany(s => s.ResumeSkills)
                   .HasForeignKey(rs => rs.SkillId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}