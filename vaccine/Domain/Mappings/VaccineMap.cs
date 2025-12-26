using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vaccine.Domain.Entities;
using vaccine.Domain.Enums;

namespace vaccine.Domain.Mappings;

public class VaccineMap : BaseEntityMap<Vaccine>
{
    public override void Configure(EntityTypeBuilder<Vaccine> builder)
    {
        builder.ToTable("vaccine");
        
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
        
        base.Configure(builder);
    }
}