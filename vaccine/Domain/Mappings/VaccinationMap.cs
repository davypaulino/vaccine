using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vaccine.Domain.Entities;

namespace vaccine.Domain.Mappings;

public class VaccionationMap : BaseEntityMap<Vaccination>
{
    
    private Guid AdminGuid = Guid.Parse("efbc569d-c8ad-463a-9158-27cdb8d8630a");
    
    public override void Configure(EntityTypeBuilder<Vaccination> builder)
    {
        builder.ToTable("vaccination");

        builder.Property(v => v.PersonId)
            .HasColumnName("person_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(v => v.VaccineId)
            .HasColumnName("vaccine_id")
            .HasColumnType("uuid")
            .IsRequired();
        
        builder.HasOne(v => v.Person)
            .WithMany(p => p.Vaccinations)
            .HasForeignKey(v => v.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(v => v.Vaccine)
            .WithMany(v => v.Vaccinations)
            .HasForeignKey(v => v.VaccineId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new
            {
                Id =  Guid.Parse("186ce810-ba7e-4ec1-84bd-094003988c3b"),
                PersonId = Guid.Parse("8cf44a3d-1700-4167-ab56-66a65c5817ba"),
                VaccineId = Guid.Parse("74c6d6e2-f3ad-4ea8-97ec-6fa900425519"),
                CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = AdminGuid
            },
            new
            {
                Id = Guid.Parse("501239da-1ccb-4540-bda4-2ccb4a89db9f"),
                PersonId = Guid.Parse("46bdd486-3de7-4977-af3a-200a7ba02773"),
                VaccineId = Guid.Parse("7def8280-c204-49ce-b04c-e3631a9e414f"),
                CreatedAt = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = AdminGuid
            }
        );
        
        base.Configure(builder);
    }
}