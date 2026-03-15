using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class SkillEntityConfiguration : BaseEntityConfiguration<Skill>
    {
        public override void Configure(EntityTypeBuilder<Skill> builder)
        {
            base.Configure(builder);

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasMany(s => s.ResumeSkills)
                   .WithOne(rs => rs.Skill)
                   .HasForeignKey(rs => rs.SkillId);

            builder.HasMany(s => s.VacancySkills)
                   .WithOne(vs => vs.Skill)
                   .HasForeignKey(vs => vs.SkillId);
        }
    }
}
