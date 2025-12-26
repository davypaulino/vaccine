namespace vaccine.Domain.Entities;

public class Vaccination : BaseEntity
{
    public Guid PersonId { get; set; }
    public Guid VaccineId { get; set; }
    
    public List<Dose> Doses { get; set; }
    public Vaccine Vaccine { get; set; }
    public Person Person { get; set; }
}