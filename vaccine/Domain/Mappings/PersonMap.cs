using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vaccine.Domain.Entities;
using vaccine.Endpoints.DTOs.Validators;

namespace vaccine.Domain.Mappings;

public class PersonMap : BaseEntityMap<Person>
{
    private Guid AdminGuid = Guid.Parse("efbc569d-c8ad-463a-9158-27cdb8d8630a");
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("person");

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.UserId)
            .HasColumnName("user_id")
            .HasDefaultValue(null);

        builder.Property(p => p.Document)
            .HasColumnName("document")
            .HasMaxLength(Cpf.LENGTH_CPF)
            .IsRequired()
            .HasConversion(
                cpf => cpf.Number,
                value => new Cpf(value)
            );

        builder.Property(p => p.Birthday)
            .HasColumnName("birthday")
            .HasColumnType("date");

        builder.HasIndex(p => p.Document)
            .IsUnique();
        
        builder.HasOne(p => p.User)
            .WithOne(u => u.Person)
            .HasForeignKey<Person>(p => p.UserId);

        builder.HasData(
            new { Id = Guid.Parse("8cf44a3d-1700-4167-ab56-66a65c5817ba"), Name = "Eduarda Marino", Document = new Cpf("99711606097"), Birthday = new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(1990, 1, 10, 0, 0, 0, DateTimeKind.Utc), CreatedBy = AdminGuid },
            new { Id = Guid.Parse("dfc67e0d-3ac4-4b81-8ec4-8ec90da29546"), Name = "João Silva", Document = new Cpf("06693964001"), Birthday = new DateTime(1985, 5, 20, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(1985, 5, 20, 0, 0, 0, DateTimeKind.Utc), CreatedBy = AdminGuid },
            new { Id = Guid.Parse("46bdd486-3de7-4977-af3a-200a7ba02773"), Name = "Maria Oliveira",Document = new Cpf("02443959007"), Birthday = new DateTime(1992, 8, 15, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(1992, 8, 15, 0, 0, 0, DateTimeKind.Utc), CreatedBy = AdminGuid },
            new { Id = Guid.Parse("8ce58e9d-81b3-4aa3-a097-396b46175865"), Name = "Carlos Pereira", Document = new Cpf("75251000049"), Birthday = new DateTime(1988, 3, 10, 0, 0, 0, DateTimeKind.Utc), CreatedAt =new DateTime(1988, 3, 10, 0, 0, 0, DateTimeKind.Utc), CreatedBy = AdminGuid },
            new { Id = Guid.Parse("322c1393-bd8e-4fb6-b074-b8764a16d317"), Name = "Ana Costa", Document = new Cpf("09712058093"), Birthday = new DateTime(1995, 11, 10, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(1995, 11, 3, 0, 0, 0, DateTimeKind.Utc), CreatedBy = AdminGuid }
        );
        
        base.Configure(builder);
    }
}