using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class CompanyMemberEntityConfiguration : BaseEntityConfiguration<CompanyMember>
    {
        public override void Configure(EntityTypeBuilder<CompanyMember> builder)
        {
            base.Configure(builder);

            builder.Property(cm => cm.Role)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasIndex(cm => new { cm.CompanyId, cm.UserId })
                   .IsUnique();

            builder.HasOne(cm => cm.Company)
                   .WithMany(c => c.Members)
                   .HasForeignKey(cm => cm.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cm => cm.User)
                   .WithMany(u => u.CompanyMembers)
                   .HasForeignKey(cm => cm.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}