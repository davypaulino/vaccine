using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using vaccine.Application.Configurations;
using vaccine.Application.Constants;
using vaccine.Application.Models;
using vaccine.Data.Entities;
using vaccine.Domain;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Responses;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace vaccine.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private const string CLASSNAME = nameof(AuthenticationService);
    private readonly VaccineDbContext _context;
    private readonly AuthenticationSettings _options;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IRequestInfo _requestInfo;
    private readonly TokenValidationParameters _tokenValidationParams;
    
    public AuthenticationService(VaccineDbContext context,
        IOptions<AuthenticationSettings> options,
        ILogger<AuthenticationService> logger,
        IRequestInfo requestInfo)
    {
        _context = context;
        _options = options.Value;
        _logger = logger;
        _requestInfo = requestInfo;
        _tokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,

            ValidateAudience = true,
            ValidAudience = _options.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(_options.SecretKey),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public async Task<Result<AuthResponse>> Authenticate(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            _logger.LogWarning("{Class} | {Method} | {UserEmail} | User don't found | {CorrelationId}",
                CLASSNAME, nameof(Authenticate), email, _requestInfo.CorrelationId);

            return Result<AuthResponse>.Failure("User don't exist.");
        }
        
        if (VerifyPassword(user.Email, user.Password, password) is false)
        {
            _logger.LogWarning("{Class} | {Method} | {UserEmail} | Password is incorrect. | {CorrelationId}",
                CLASSNAME, nameof(Authenticate), email, _requestInfo.CorrelationId);

            return Result<AuthResponse>.Failure("User or Password are incorrect.");
        }
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(VaccineClaimTypes.PersonId, user.PersonId.ToString()),
        };
        
        var key = new SymmetricSecurityKey(_options.SecretKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        _logger.LogInformation("{Class} | {Method} | {UserEmail} | User authenticated. | {CorrelationId}",
            CLASSNAME, nameof(Authenticate), email, _requestInfo.CorrelationId);

        return Result<AuthResponse>.Ok(new AuthResponse(tokenString, expires, null));
    }
    
    public async Task<Result<AuthResponse>> Register(RegisterRequest request)
    {
        var exists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (exists)
        {
            _logger.LogWarning(
                "{Class} | {Method} | {UserEmail} | User already exists | {CorrelationId}",
                CLASSNAME, nameof(Register), request.Email, _requestInfo.CorrelationId);

            return Result<AuthResponse>.Failure("User already exists.");
        }
        
        Guid.TryParse(request.PersonId, out Guid personId);
        var user = new User()
        {
            PersonId = personId,
            Email = request.Email,
            Password = HashPassword(request.Email, request.Password),
            Role = request.Role,
            Status = request.Status,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "{Class} | {Method} | {UserEmail} | User registered | {CorrelationId}",
            CLASSNAME, nameof(Register), request.Email, _requestInfo.CorrelationId);
        
        return await Authenticate(request.Email, request.Password);
    }

    public Task<Result<AuthResponse>> RefreshToken(string refreshToken)
    {
        throw new Exception();
    }

    public Result<ClaimsPrincipal?> GetPrincipalFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("{Class} | {Method} | Token is empty | {CorrelationId}",
                CLASSNAME, nameof(GetPrincipalFromToken), _requestInfo.CorrelationId);

            return Result<ClaimsPrincipal?>.Failure("Token is empty.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParams, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("{Class} | {Method} | Invalid token algorithm | Alg: {Alg} | {CorrelationId}",
                    CLASSNAME, nameof(GetPrincipalFromToken), (validatedToken as JwtSecurityToken)?.Header.Alg, _requestInfo.CorrelationId);

                return Result<ClaimsPrincipal?>.Failure("Invalid token.");
            }

            _logger.LogInformation("{Class} | {Method} | Token successfully validated | Subject: {Subject} | {CorrelationId}",
                CLASSNAME, nameof(GetPrincipalFromToken), principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, _requestInfo.CorrelationId);
            
            return Result<ClaimsPrincipal?>.Ok(principal);
        }
        catch (SecurityTokenExpiredException)
        {
            _logger.LogWarning("{Class} | {Method} | Token expired | {CorrelationId}",
                CLASSNAME, nameof(GetPrincipalFromToken), _requestInfo.CorrelationId);

            return Result<ClaimsPrincipal?>.Failure("Token expired.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Class} | {Method} | Invalid token | {CorrelationId}",
                CLASSNAME, nameof(GetPrincipalFromToken), _requestInfo.CorrelationId);

            return Result<ClaimsPrincipal?>.Failure("Invalid token.");
        }
    }

    public static string HashPassword(string email, string password)
    {
        var hasher = new PasswordHasher<string>();
        return hasher.HashPassword(email, password);
    }

    public static bool VerifyPassword(string email, string hashedPassword, string password)
    {
        var hasher = new PasswordHasher<string>();

        var result =  hasher.VerifyHashedPassword(email, hashedPassword, password);
        return result == PasswordVerificationResult.Success;
    }
}