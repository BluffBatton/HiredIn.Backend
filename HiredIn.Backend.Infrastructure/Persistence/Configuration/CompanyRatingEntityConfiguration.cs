using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    public class CompanyRatingEntityConfiguration : BaseEntityConfiguration<CompanyRating>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CompanyRating> builder)
        {
            base.Configure(builder);
            builder.Property(cr => cr.Rating)
                   .IsRequired();
            builder.HasOne(cr => cr.Company)
                   .WithMany(c => c.CompanyRatings)
                   .HasForeignKey(cr => cr.CompanyId)
                   .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
            builder.HasOne(cr => cr.User)
                   .WithMany(u => u.CompanyRatings)
                   .HasForeignKey(cr => cr.UserId)
                   .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
        }
    }
}
