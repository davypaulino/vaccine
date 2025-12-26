using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vaccine.Domain.Entities;

namespace vaccine.Domain.Mappings;

public abstract class BaseEntityMap<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    private IEntityTypeConfiguration<BaseEntity> _entityTypeConfigurationImplementation;

    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .HasColumnName("id")
            .HasColumnType("uuid")
            .IsRequired();
        
        builder.Property(b => b.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(b => b.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(b => b.UpdatedBy)
            .HasColumnName("updated_by");
    }
}