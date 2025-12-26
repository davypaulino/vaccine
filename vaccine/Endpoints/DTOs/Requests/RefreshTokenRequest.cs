namespace vaccine.Endpoints.DTOs.Requests;

public record RefreshTokenRequest(string token, string refreshToken);