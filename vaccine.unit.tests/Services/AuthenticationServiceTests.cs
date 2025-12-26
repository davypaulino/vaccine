using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EntityFrameworkCore.Testing.Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using vaccine.Application.Configurations;
using vaccine.Application.Constants;
using vaccine.Application.Services;
using vaccine.Data;
using vaccine.Data.Entities;
using vaccine.Domain;
using vaccine.Domain.Enums;
using vaccine.Endpoints.DTOs.Requests;

namespace vaccine.unit.tests.Services;

public class AuthenticationServiceTests
{
    private readonly VaccineDbContext _contextMock;
    private readonly Mock<ILogger<AuthenticationService>> _loggerMock;
    private readonly Mock<IRequestInfo> _requestInfoMock;
    private readonly IOptions<AuthenticationSettings> _options;

    public AuthenticationServiceTests()
    {
        _contextMock = Create.MockedDbContextFor<VaccineDbContext>();
        _loggerMock = new Mock<ILogger<AuthenticationService>>();
        _requestInfoMock = new Mock<IRequestInfo>();

        _requestInfoMock.Setup(x => x.CorrelationId)
            .Returns(Guid.NewGuid());

        _options = Options.Create(new AuthenticationSettings
        {
            SecretKeyValue = "super-secret-jwt-key-with-32-chars!",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpiryMinutes = 60
        });
    }
    
    private string GenerateToken(
        IEnumerable<Claim> claims,
        DateTime expires,
        string algorithm = SecurityAlgorithms.HmacSha256)
    {
        var key = new SymmetricSecurityKey(_options.Value.SecretKey);

        var creds = new SigningCredentials(key, algorithm);

        var token = new JwtSecurityToken(
            issuer: _options.Value.Issuer,
            audience: _options.Value.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    private AuthenticationService CreateService(List<User> users)
    {
        _contextMock.Users.RemoveRange(_contextMock.Users);
        _contextMock.Users.AddRange(users);
        _contextMock.SaveChanges();

        return new AuthenticationService(
            _contextMock,
            _options,
            _loggerMock.Object,
            _requestInfoMock.Object
        );
    }

    [Fact]
    public async Task Authenticate_WhenUserDoesNotExist_ShouldReturnError()
    {
        var service = CreateService(new List<User>());

        var result = await service.AuthenticateAsync("test@email.com", "123", CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User don't exist.", result.Error);
    }

    [Fact]
    public async Task Authenticate_WhenPasswordIsInvalid_ShouldReturnError()
    {
        var email = "test@email.com";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Password = AuthenticationService.HashPassword(email, "correct-password"),
            Role = ERole.Person,
            Status = EStatus.Active
        };

        var service = CreateService(new List<User> { user });

        var result = await service.AuthenticateAsync(user.Email, "wrong-password", CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User or Password are incorrect.", result.Error);
    }

    [Fact]
    public async Task Authenticate_WithValidCredentials_ShouldReturnJwt()
    {
        var pass = "123456";
        var email = "test@email.com";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Password = AuthenticationService.HashPassword(email, pass),
            Role = ERole.Person,
            Status = EStatus.Active,
            PersonId = Guid.NewGuid()
        };

        var service = CreateService(new List<User> { user });

        var result = await service.AuthenticateAsync(user.Email, pass, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data!.token);
        Assert.NotEmpty(result.Data!.token);
        
        var handler = new JwtSecurityTokenHandler();
        var jwt =  handler.ReadJwtToken(result.Data!.token);

        Assert.Equal("test-issuer", jwt.Issuer);
        Assert.Contains("test-audience", jwt.Audiences);
        
        var claims = jwt.Claims.ToDictionary(c => c.Type, c => c.Value);
        Assert.Equal(user.Id.ToString(), claims[JwtRegisteredClaimNames.Sub]);
        Assert.Equal(((int)ERole.Person).ToString(), claims[ClaimTypes.Role]);
        Assert.Equal(email, claims[ClaimTypes.Email]);
    }
    
    [Fact]
    public async Task Authenticate_WithValidCredentials_ShouldHaveCorrectExpiration()
    {
        var email = "expiry@test.com";
        var pass = "123456";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Password = AuthenticationService.HashPassword(email, pass),
            Role = ERole.Person,
            Status = EStatus.Active,
            PersonId = Guid.NewGuid()
        };

        var service = CreateService(new List<User> { user });

        var before = DateTime.UtcNow;
        var result = await service.AuthenticateAsync(email, pass, CancellationToken.None);
        var after = DateTime.UtcNow;

        var handler = new JwtSecurityTokenHandler();
        var jwt =  handler.ReadJwtToken(result.Data!.token);

        Assert.NotNull(jwt.ValidTo);
        Assert.InRange(jwt.ValidTo, 
            before.AddMinutes(59), 
            after.AddMinutes(61));
    }

    [Fact]
    public async Task Register_WhenUserAlreadyExists_ShouldReturnError()
    {
        var email = "test@email.com";
        var pass = "123456";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Password = AuthenticationService.HashPassword(email, pass),
            Role = ERole.Admin,
            Status = EStatus.Active,
            PersonId = Guid.NewGuid()
        };
        
        var service = CreateService(new List<User> { user });

        var request = new RegisterRequest(email,
            pass,
            null,
            ERole.Person,
            EStatus.Active
        );

        var result = await service.RegisterAsync(request, CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal("User already exists.", result.Error);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldCreateUserAndAuthenticate()
    {
        var service = CreateService(new List<User>());

        var request = new RegisterRequest("new@email.com",
            "123456",
            null,
            ERole.Person,
            EStatus.Active);

        var result = await service.RegisterAsync(request, CancellationToken.None);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.Data!.token);
        Assert.NotEmpty(result.Data!.token);
        Assert.NotNull(_contextMock.Users.FirstOrDefault(x => x.Email == request.Email));
    }
    
    [Fact]
    public async Task Register_WithValidData_ShouldStoreHashedPassword()
    {
        var service = CreateService(new List<User>());

        var request = new RegisterRequest(
            "hash@test.com",
            "123456",
            null,
            ERole.Person,
            EStatus.Active
        );

        await service.RegisterAsync(request, CancellationToken.None);

        var user = _contextMock.Users.First(u => u.Email == request.Email);

        Assert.NotEqual("123456", user.Password);
        Assert.True(
            AuthenticationService.VerifyPassword(
                user.Email,
                user.Password,
                "123456"
            )
        );
    }
}