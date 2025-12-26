namespace vaccine.Endpoints.DTOs.Responses;

public record PersonResponse(Guid Id, string Name, string Document,  DateTime BirthDate);