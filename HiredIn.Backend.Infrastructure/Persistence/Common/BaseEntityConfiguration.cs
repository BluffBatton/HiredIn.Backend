using HiredIn.Backend.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HiredIn.Backend.Infrastructure.Persistence.Common
{
    public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedAtUtc)
                   .IsRequired();
            builder.Property(e => e.UpdatedAtUtc);
            builder.Property(e => e.DeletedAtUtc);
        }

    }    
}