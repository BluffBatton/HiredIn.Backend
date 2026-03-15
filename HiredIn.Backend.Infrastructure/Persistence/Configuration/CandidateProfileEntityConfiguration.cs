using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class CandidateProfileEntityConfiguration : BaseEntityConfiguration<CandidateProfile>
    {
        public override void Configure(EntityTypeBuilder<CandidateProfile> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.City)
                   .HasMaxLength(200);

            builder.Property(c => c.About)
                   .HasMaxLength(2000);

            builder.Property(c => c.OpenToWork)
                   .IsRequired();

            builder.HasIndex(c => c.UserId)
                   .IsUnique();

            builder.HasOne(c => c.User)
                   .WithOne(u => u.CandidateProfile)
                   .HasForeignKey<CandidateProfile>(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}