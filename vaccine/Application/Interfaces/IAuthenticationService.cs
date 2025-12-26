using System.Security.Claims;
using vaccine.Application.Models;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Responses;

namespace vaccine.Application.Configurations;

public interface IAuthenticationService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<Result<AuthResponse>> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
    Task<Result<AuthResponse>> RefreshToken(string refreshToken);
}