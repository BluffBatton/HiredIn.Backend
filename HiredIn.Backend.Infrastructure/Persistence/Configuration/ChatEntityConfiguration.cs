using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class ChatEntityConfiguration : BaseEntityConfiguration<Chat>
    {
        public override void Configure(EntityTypeBuilder<Chat> builder)
        {
            base.Configure(builder);

            builder.ToTable("Chats");

            builder.Property(c => c.Status)
                   .IsRequired();

            builder.HasOne(c => c.Application)
                   .WithOne(a => a.Chat)
                   .HasForeignKey<Chat>(c => c.ApplicationId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Participants)
                   .WithOne(p => p.Chat)
                   .HasForeignKey(p => p.ChatId);

            builder.HasMany(c => c.Messages)
                   .WithOne(m => m.Chat)
                   .HasForeignKey(m => m.ChatId);
        }
    }
}
