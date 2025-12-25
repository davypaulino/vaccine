using vaccine.Data.Enums;

namespace vaccine.Endpoints.DTOs.Requests;

public record ModifyVaccineRequest(string Name, EDoseType[] AvailableDoses);