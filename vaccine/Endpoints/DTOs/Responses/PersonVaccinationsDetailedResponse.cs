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
    public EDoseType DosesTaken { get; set; }
}

public class PersonVaccinationsDetailedResponse
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Document { get; set; }
    public HashSet<VaccinationResponse> Vaccinations { get; set; }
}