using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class FavouriteVacancyEntityConfiguration : BaseEntityConfiguration<FavouriteVacancy>
    {
        public override void Configure(EntityTypeBuilder<FavouriteVacancy> builder)
        {
            base.Configure(builder);

            builder.ToTable("FavouriteVacancies");

            builder.HasKey(fv => new { fv.UserId, fv.VacancyId });

            builder.HasOne(fv => fv.User)
                   .WithMany(u => u.FavouriteVacancies)
                   .HasForeignKey(fv => fv.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fv => fv.Vacancy)
                   .WithMany(v => v.FavouriteVacancies)
                   .HasForeignKey(fv => fv.VacancyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
