using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vaccine.Domain.Entities;

namespace vaccine.Domain.Mappings;

public class VaccionationMap : BaseEntityMap<Vaccination>
{
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
        
        base.Configure(builder);
    }
}