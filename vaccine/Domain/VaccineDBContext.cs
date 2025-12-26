using Microsoft.EntityFrameworkCore;
using vaccine.Data.Entities;
using vaccine.Domain.Entities;

namespace vaccine.Domain;

public class VaccineDbContext(DbContextOptions<VaccineDbContext> options) : DbContext(options)
{
    public virtual DbSet<Vaccine> Vaccines { get; set; }
    public virtual DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(VaccineDbContext).Assembly);
}