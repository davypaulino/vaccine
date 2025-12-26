namespace vaccine.Endpoints.DTOs.Requests;

public record AuthenticateRequest(string Email, string Password);