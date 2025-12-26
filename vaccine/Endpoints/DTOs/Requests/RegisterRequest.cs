using vaccine.Domain.Enums;

namespace vaccine.Endpoints.DTOs.Requests;

public record RegisterRequest(string Email, string Password, string Document);