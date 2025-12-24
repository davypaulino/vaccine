using vaccine.Data.Enums;

namespace vaccine.Endpoints.DTOs.Requests;

public record CreateVaccineRequest(string Name, EDoseType[] AvailableEDoses);