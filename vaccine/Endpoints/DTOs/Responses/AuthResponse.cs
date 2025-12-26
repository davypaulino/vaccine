namespace vaccine.Endpoints.DTOs.Responses;

public record AuthResponse(string token, DateTime expires, string? refreshToken);