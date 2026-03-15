using HiredIn.Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class ResumeWorkExperienceEntityConfiguration : IEntityTypeConfiguration<ResumeWorkExperience>
    {
        public void Configure(EntityTypeBuilder<ResumeWorkExperience> builder)
        {
            builder.HasKey(w => new { w.ResumeId, w.CompanyName, w.PositionTitle });

            builder.Property(w => w.CompanyName)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(w => w.PositionTitle)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(w => w.Description)
                   .HasMaxLength(2000);

            builder.Property(w => w.IsCurrent)
                   .IsRequired();

            builder.HasOne(w => w.Resume)
                   .WithMany(r => r.WorkExperiences)
                   .HasForeignKey(w => w.ResumeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}