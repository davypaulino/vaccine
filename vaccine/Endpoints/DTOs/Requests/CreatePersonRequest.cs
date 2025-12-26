using System.Text.Json.Serialization;

namespace vaccine.Endpoints.DTOs.Requests;

public record CreatePersonRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("document")] string Document,
    [property: JsonPropertyName("birthDate")] DateTime BirthDate
);