using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vaccine.Data.Entities;
using vaccine.Data.Enums;

namespace vaccine.Data.Mappings;

public class VaccineMap : IEntityTypeConfiguration<Vaccine>
{
    public void Configure(EntityTypeBuilder<Vaccine> builder)
    {
        builder.ToTable("vaccine");
        
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasColumnName("id")
            .IsRequired();
        
        builder.Property(v => v.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(v => v.AvailableTypes)
            .HasColumnName("available_types")
            .HasColumnType("smallint")
            .IsRequired()
            .HasConversion(
                v => (int)v,
                v => (EDoseType)v
            );
        
        builder.Property(v => v.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(v => v.CreatedBy)
            .HasColumnName("created_by")
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(v => v.UpdatedBy)
            .HasColumnName("updated_by");
    }
}