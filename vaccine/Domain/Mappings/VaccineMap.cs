using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vaccine.Domain.Entities;
using vaccine.Domain.Enums;

namespace vaccine.Domain.Mappings;

public class VaccineMap : BaseEntityMap<Vaccine>
{
    private readonly Guid AdminGuid = Guid.Parse("efbc569d-c8ad-463a-9158-27cdb8d8630a");
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
        
        builder.HasData(
            new { Id = Guid.Parse("74c6d6e2-f3ad-4ea8-97ec-6fa900425519"), Name = "Hepatite B",
                AvailableTypes = EDoseType.First | EDoseType.Second | EDoseType.Third, CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), CreatedBy = AdminGuid },
            new { Id = Guid.Parse("7def8280-c204-49ce-b04c-e3631a9e414f"), Name = "COVID-19",
                AvailableTypes = EDoseType.First | EDoseType.Second | EDoseType.FirstReinforcement | EDoseType.SecondReinforcement, CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), CreatedBy = AdminGuid },
            new { Id = Guid.Parse("63f8f8de-1772-4220-ab07-d00f3748a4c9"), Name = "Influenza (Gripe)", 
                AvailableTypes = EDoseType.First, CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), CreatedBy = AdminGuid },
            new { Id = Guid.Parse("05df7267-b874-42b2-944d-7c04c100d85e"), Name = "Febre Amarela",
                AvailableTypes = EDoseType.First, CreatedAt =  new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), CreatedBy = AdminGuid },
            new { Id = Guid.Parse("45e171d4-e07a-42c9-973d-68d947b86b9f"), Name = "Tríplice Viral (Sarampo, Caxumba, Rubéola)",
                AvailableTypes = EDoseType.First | EDoseType.Second, CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc), CreatedBy = AdminGuid }
        );
        
        base.Configure(builder);
    }
}