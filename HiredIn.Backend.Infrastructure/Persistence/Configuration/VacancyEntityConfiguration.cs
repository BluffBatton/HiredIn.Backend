using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class VacancyEntityConfiguration : BaseEntityConfiguration<Vacancy>
    {
        public override void Configure(EntityTypeBuilder<Vacancy> builder)
        {
            base.Configure(builder);

            builder.Property(v => v.Title)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(v => v.Description)
                   .IsRequired()
                   .HasMaxLength(5000);

            builder.Property(v => v.City)
                   .HasMaxLength(200);

            builder.HasOne(v => v.Company)
                   .WithMany(c => c.Vacancies)
                   .HasForeignKey(v => v.CompanyId)
                   .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

            builder.HasMany(v => v.VacancySkills)
                   .WithOne(vs => vs.Vacancy)
                   .HasForeignKey(vs => vs.VacancyId);

            builder.HasMany(v => v.Applications)
                   .WithOne(a => a.Vacancy)
                   .HasForeignKey(a => a.VacancyId);

            builder.HasMany(v => v.FavouriteVacancies)
                   .WithOne(fv => fv.Vacancy)
                   .HasForeignKey(fv => fv.VacancyId);
        }
    }
}
