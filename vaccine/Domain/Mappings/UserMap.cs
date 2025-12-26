using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using vaccine.Data.Entities;
using vaccine.Domain.Enums;

namespace vaccine.Domain.Mappings;

public class UserMap : BaseEntityMap<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");
        
        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(150)
            .IsRequired();
        
        builder.Property(u => u.Password)
            .HasColumnName("password")
            .HasColumnType("text")
            .IsRequired();
        
        builder.Property(u => u.Role)
            .HasColumnName("role")
            .HasColumnType("smallint")
            .IsRequired()
            .HasConversion(
                u => (int)u,
                u => (ERole)u
            );

        builder.Property(u => u.Status)
            .HasColumnName("status")
            .HasColumnType("smallint")
            .IsRequired()
            .HasConversion(
                u => (int)u,
                u => (EStatus)u
            );
        
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        base.Configure(builder);
    }
}
