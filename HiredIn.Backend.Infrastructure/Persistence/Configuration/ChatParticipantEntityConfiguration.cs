using HiredIn.Backend.Domain.Entities;
using HiredIn.Backend.Infrastructure.Persistence.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Configuration
{
    internal class ChatParticipantEntityConfiguration : BaseEntityConfiguration<ChatParticipant>
    {
        public override void Configure(EntityTypeBuilder<ChatParticipant> builder)
        {
            base.Configure(builder);

            builder.ToTable("ChatParticipants");

            builder.HasKey(cp => new { cp.ChatId, cp.UserId });

            builder.HasOne(cp => cp.Chat)
                   .WithMany(c => c.Participants)
                   .HasForeignKey(cp => cp.ChatId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.User)
                   .WithMany(u => u.ChatParticipants)
                   .HasForeignKey(cp => cp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
