namespace vaccine.Application.Configurations;

public interface IRequestInfo
{
    Guid? UserId { get; }

    string? Name { get; }

    string? EmailAddress { get; }

    string? IP { get; }

    string? Port { get; }

    Guid CorrelationId { get; }

    void SetUserInfo(
        Guid? userId,
        string? name,
        string? email);

    void SetIP(string ip);

    void SetPort(string port);

    void SetCorrelationId(Guid correlationId);
}