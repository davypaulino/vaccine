using vaccine.Domain.Enums;

namespace vaccine.Application.Configurations;

public interface IRequestInfo
{
    Guid? UserId { get; }

    string? Name { get; }

    string? EmailAddress { get; }
    
    ERole? Role { get; }

    string? IP { get; }

    string? Port { get; }

    Guid CorrelationId { get; }

    public void SetUserInfo(
        string? userId,
        string? name,
        string? email,
        string role);

    void SetIP(string ip);

    void SetPort(string port);

    void SetCorrelationId(Guid correlationId);
}