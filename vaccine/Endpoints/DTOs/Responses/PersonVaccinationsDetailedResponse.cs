using vaccine.Domain.Enums;

namespace vaccine.Endpoints.DTOs.Responses;

public class DoseResponse
{
    public EDoseType DoseType { get; set; }
    public DateTime AppliedAt { get; set; }
}

public class VaccinationResponse
{
    public string VaccineName { get; set; } 
    public HashSet<DoseResponse> Doses { get; set; }
    public EDoseType AvailableDoses { get; set; }
    
    public bool Equals(VaccinationResponse other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return VaccineName == other.VaccineName;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as VaccinationResponse);
    }

    public override int GetHashCode()
    {
        return VaccineName?.GetHashCode() ?? 0;
    }
}

public class PersonVaccinationsDetailedResponse
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Document { get; set; }
    public HashSet<VaccinationResponse> Vaccinations { get; set; }
}