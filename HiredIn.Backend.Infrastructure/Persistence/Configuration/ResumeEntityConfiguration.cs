using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class ResumeEntityConfiguration : BaseEntityConfiguration<Resume>
    {
        public override void Configure(EntityTypeBuilder<Resume> builder)
        {
            base.Configure(builder);

            builder.Property(r => r.Title)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(r => r.DesiredPosition)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(r => r.Summary)
                   .HasMaxLength(2000);

            builder.Property(r => r.EmploymentType)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(r => r.WorkFormat)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(r => r.ExperienceLevel)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(r => r.Visibility)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(r => r.IsPrimary)
                   .IsRequired();

            builder.HasOne(r => r.CandidateProfile)
                   .WithMany(cp => cp.Resumes)
                   .HasForeignKey(r => r.CandidateProfileId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}