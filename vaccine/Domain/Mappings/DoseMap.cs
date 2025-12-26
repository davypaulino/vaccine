using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vaccine.Domain.Entities;
using vaccine.Domain.Enums;

namespace vaccine.Domain.Mappings;

public class DoseMap : BaseEntityMap<Dose>
{
    public override void Configure(EntityTypeBuilder<Dose> builder)
    {
        builder.ToTable("dose");

        builder.Property(d => d.VaccinationId)
            .HasColumnName("vaccination_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(d => d.AppliedAt)
            .HasColumnName("applied_at")
            .IsRequired();
        
        builder.Property(v => v.DoseType)
            .HasColumnName("dose_type")
            .HasColumnType("smallint")
            .IsRequired()
            .HasConversion(
                v => (int)v,
                v => (EDoseType)v
            );

        builder.HasOne(d => d.Vaccination)
            .WithMany(v => v.Doses)
            .HasForeignKey(d => d.VaccinationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.VaccinationId);
        
        base.Configure(builder);
    }
}