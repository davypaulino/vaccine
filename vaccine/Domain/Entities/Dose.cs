using vaccine.Domain.Enums;

namespace vaccine.Domain.Entities;

public class Dose : BaseEntity
{
    public Guid VaccinationId { get; set; } 
    public EDoseType DoseType { get; set; }
    public DateTime AppliedAt { get; set; }
    
    public Vaccination Vaccination { get; set; }
}