using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class ResumeEducationEntityConfiguration : BaseEntityConfiguration<ResumeEducation>
    {
        public override void Configure(EntityTypeBuilder<ResumeEducation> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.InstitutionName)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(e => e.Degree)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.FieldOfStudy)
                   .HasMaxLength(200);

            builder.Property(e => e.Description)
                   .HasMaxLength(2000);

            builder.HasOne(e => e.Resume)
                   .WithMany(r => r.Educations)
                   .HasForeignKey(e => e.ResumeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}