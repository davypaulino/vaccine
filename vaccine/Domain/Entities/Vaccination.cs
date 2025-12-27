namespace vaccine.Domain.Entities;

public class Vaccination : BaseEntity
{
    public Guid PersonId { get; set; }
    public Guid VaccineId { get; set; }
    
    public HashSet<Dose> Doses { get; set; } = new();
    public Vaccine Vaccine { get; set; }
    public Person Person { get; set; }
}