using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class UserEntityConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.Property(u => u.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.LastName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PhoneNumber)
                   .HasMaxLength(30);

            builder.Property(u => u.PasswordHash)
                   .IsRequired();

            builder.Property(u => u.AvatarUrl)
                   .HasMaxLength(1000);

            builder.HasOne(u => u.CandidateProfile)
                   .WithOne(cp => cp.User)
                   .HasForeignKey<CandidateProfile>(cp => cp.UserId)
                   .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);

            builder.HasMany(u => u.CompanyMembers)
                   .WithOne(cm => cm.User)
                   .HasForeignKey(cm => cm.UserId);

            builder.HasMany(u => u.SentMessages)
                   .WithOne(m => m.SenderUser)
                   .HasForeignKey(m => m.SenderUserId);

            builder.HasMany(u => u.Notifications)
                   .WithOne(n => n.User)
                   .HasForeignKey(n => n.UserId);

            builder.HasMany(u => u.FavouriteVacancies)
                   .WithOne(fv => fv.User)
                   .HasForeignKey(fv => fv.UserId);

            builder.HasMany(u => u.ChatParticipants)
                   .WithOne(cp => cp.User)
                   .HasForeignKey(cp => cp.UserId);
        }
    }
}
