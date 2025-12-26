using System.Security.Claims;
using vaccine.Application.Models;
using vaccine.Endpoints.DTOs.Responses;

namespace vaccine.Application.Configurations;

public interface IAuthenticationService
{
    Task<Result<AuthResponse>> Authenticate(string username, string password);
    Task<Result<AuthResponse>> RefreshToken(string refreshToken);
    Result<ClaimsPrincipal?> GetPrincipalFromToken(string token);
}