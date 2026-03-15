using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class ResumeFileEntityConfiguration : BaseEntityConfiguration<ResumeFile>
    {
        public override void Configure(EntityTypeBuilder<ResumeFile> builder)
        {
            base.Configure(builder);

            builder.Property(f => f.FileName)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(f => f.FileUrl)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.HasOne(f => f.Resume)
                   .WithOne(r => r.ResumeFile)
                   .HasForeignKey<ResumeFile>(f => f.ResumeId)
                   .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
        }
    }
}
