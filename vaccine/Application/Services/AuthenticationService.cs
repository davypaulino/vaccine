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
using vaccine.Domain.Enums;
using vaccine.Endpoints.DTOs.Requests;
using vaccine.Endpoints.DTOs.Responses;
using vaccine.Endpoints.DTOs.Validators;
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

    public async Task<Result<AuthResponse>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
    {
        if (email == _options.AdminName && password == _options.AdminPassword)
        {
            var adminEmail = "admin@email.com";
            var tokenByPass = GenerateToken(_requestInfo.CorrelationId, ERole.Admin, adminEmail, null);
        
            _logger.LogInformation("{Class} | {Method} | {UserEmail} | User authenticated. | {CorrelationId}",
                CLASSNAME, nameof(AuthenticateAsync), adminEmail, _requestInfo.CorrelationId);

            return Result<AuthResponse>.Ok(new AuthResponse(tokenByPass.Item1, tokenByPass.Item2, null, _requestInfo.CorrelationId));
        }
        
        var user = await _context.Users
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        
        if (user is null)
        {
            _logger.LogWarning("{Class} | {Method} | {UserEmail} | User don't found | {CorrelationId}",
                CLASSNAME, nameof(AuthenticateAsync), email, _requestInfo.CorrelationId);

            return Result<AuthResponse>.Failure("User don't exist.");
        }
        
        if (VerifyPassword(user.Email, user.Password, password) is false)
        {
            _logger.LogWarning("{Class} | {Method} | {UserId} | {UserEmail} | Password is incorrect. | {CorrelationId}",
                CLASSNAME, nameof(AuthenticateAsync), user.Id, email, _requestInfo.CorrelationId);

            return Result<AuthResponse>.Failure("User or Password are incorrect.");
        }

        var token = GenerateToken(user.Id, user.Role, user.Email, user.Person?.Id);
        
        _logger.LogInformation("{Class} | {Method} | {UserId} | {UserEmail} | User authenticated. | {CorrelationId}",
            CLASSNAME, nameof(AuthenticateAsync), user.Id, email, _requestInfo.CorrelationId);

        return Result<AuthResponse>.Ok(new AuthResponse(token.Item1, token.Item2, null, user.Id));
    }

    private (string, DateTime)  GenerateToken(
        Guid userId,
        ERole role,
        string email,
        Guid? personId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(ClaimTypes.Role, ((int)role).ToString()),
            new Claim(ClaimTypes.Email, email),
        };

        if (personId is not null)
            claims.Append(new Claim(VaccineClaimTypes.PersonId, personId.ToString()));
        
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

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
    
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var cpf = new Cpf(request.Document);
        var person = await _context.Persons
            .FirstOrDefaultAsync(p => p.Document == cpf, cancellationToken);
        
        if (person is null)
        {
            _logger.LogWarning(
                "{Class} | {Method} | {UserEmail} | User can't register like a Person user | {CorrelationId}",
                CLASSNAME, nameof(RegisterAsync), request.Email, _requestInfo.CorrelationId);

            return Result<AuthResponse>.Failure("User don't has registers.");
        }
        
        var exists = await _context.Users
            .Include(u => u.Person)
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (exists)
        {
            _logger.LogWarning(
                "{Class} | {Method} | {UserEmail} | User already exists | {CorrelationId}",
                CLASSNAME, nameof(RegisterAsync), request.Email, _requestInfo.CorrelationId);

            return Result<AuthResponse>.Failure("User already exists.");
        }
        
        var user = new User()
        {
            Email = request.Email,
            Password = HashPassword(request.Email, request.Password),
            Role = ERole.Person,
            Status = EStatus.Active,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        
        person.UserId = user.Id;
        _context.Update(person);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "{Class} | {Method} | {UserEmail} | User registered | {CorrelationId}",
            CLASSNAME, nameof(RegisterAsync), request.Email, _requestInfo.CorrelationId);
        
        return await AuthenticateAsync(request.Email, request.Password, cancellationToken);
    }

    public Task<Result<AuthResponse>> RefreshToken(string refreshToken)
    {
        throw new Exception();
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