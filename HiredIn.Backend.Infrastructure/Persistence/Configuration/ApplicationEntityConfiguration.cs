using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationEntity = HiredIn.Backend.Domain.Entities.Application;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class ApplicationEntityConfiguration : BaseEntityConfiguration<ApplicationEntity>
    {
        public override void Configure(EntityTypeBuilder<ApplicationEntity> builder)
        {
            base.Configure(builder);

            builder.Property(a => a.CoverLetter)
                   .HasMaxLength(2000);

            builder.Property(a => a.Status)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasOne(a => a.Vacancy)
                   .WithMany(v => v.Applications)
                   .HasForeignKey(a => a.VacancyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Resume)
                   .WithMany(r => r.Applications)
                   .HasForeignKey(a => a.ResumeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}