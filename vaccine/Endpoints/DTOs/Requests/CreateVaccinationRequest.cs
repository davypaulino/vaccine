using vaccine.Domain.Enums;

namespace vaccine.Endpoints.DTOs.Requests;

public class DosesResponse
{
    public DateTime AppliedAt { get; set; }
    public EDoseType DoseType { get; set; }
    
    public bool Equals(DosesResponse other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return DoseType == other.DoseType;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as DosesResponse);
    }

    public override int GetHashCode()
    {
        return DoseType.GetHashCode();
    }
}

public class CreateVaccinationRequest
{
    public Guid PersonId { get; set; }
    public Guid VaccineId { get; set; }
    public HashSet<DosesResponse> Doses { get; set; }
}