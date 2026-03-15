using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class CompanyEntityConfiguration : BaseEntityConfiguration<Company>
    {
        public override void Configure(EntityTypeBuilder<Company> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(c => c.Description)
                   .HasMaxLength(2000);

            builder.Property(c => c.Industry)
                   .HasMaxLength(200);

            builder.Property(c => c.City)
                   .HasMaxLength(200);

            builder.Property(c => c.Website)
                   .HasMaxLength(1000);

            builder.Property(c => c.LogoUrl)
                   .HasMaxLength(1000);

            builder.Property(c => c.Status)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}