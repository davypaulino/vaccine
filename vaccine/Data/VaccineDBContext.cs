using Microsoft.EntityFrameworkCore;
using vaccine.Data.Entities;

namespace vaccine.Data;

public class VaccineDBContext(DbContextOptions<VaccineDBContext> options) : DbContext(options)
{
    public virtual DbSet<Vaccine> Vaccines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(VaccineDBContext).Assembly);
}