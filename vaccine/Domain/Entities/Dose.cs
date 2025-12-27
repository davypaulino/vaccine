using vaccine.Domain.Enums;

namespace vaccine.Domain.Entities;

public class Dose : BaseEntity
{
    public Dose(){}
    
    public Dose(Guid vaccinationId, EDoseType doseType, DateTime appliedAt)
    {
        VaccinationId = vaccinationId;
        DoseType = doseType;
        AppliedAt = appliedAt;
    }
    
    public bool Equals(Dose other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return DoseType == other.DoseType && VaccinationId == other.VaccinationId;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Dose);
    }

    public override int GetHashCode()
    {
        return DoseType.GetHashCode() | VaccinationId.GetHashCode();
    }
    
    public Guid VaccinationId { get; set; } 
    public EDoseType DoseType { get; set; }
    public DateTime AppliedAt { get; set; }
    
    public Vaccination Vaccination { get; set; }
}