using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vaccine.Domain.Entities;
using vaccine.Domain.Enums;

namespace vaccine.Domain.Mappings;

public class DoseMap : BaseEntityMap<Dose>
{
    private Guid AdminGuid = Guid.Parse("efbc569d-c8ad-463a-9158-27cdb8d8630a");
    
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
        
        builder.HasData(
            new
            {
                Id = Guid.Parse("306b5c29-77b7-4d28-b5df-08007d88a8d6"),
                VaccinationId = Guid.Parse("186ce810-ba7e-4ec1-84bd-094003988c3b"),
                DoseType = EDoseType.First,
                AppliedAt = new DateTime(2024, 01, 10, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 01, 10, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = AdminGuid
            },
            new
            {
                Id = Guid.Parse("8cc63d6b-ca5d-4136-8870-26dd0afd0ca0"),
                VaccinationId = Guid.Parse("186ce810-ba7e-4ec1-84bd-094003988c3b"),
                DoseType = EDoseType.Second,
                AppliedAt = new DateTime(2024, 02, 10, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 02, 10, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = AdminGuid
            },
            new
            {
                Id = Guid.Parse("10df3852-b344-40d0-acb3-b88952419bef"),
                VaccinationId = Guid.Parse("501239da-1ccb-4540-bda4-2ccb4a89db9f"),
                DoseType = EDoseType.First,
                AppliedAt = new DateTime(2024, 03, 05, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 03, 05, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = AdminGuid
            }
        );
        
        base.Configure(builder);
    }
}