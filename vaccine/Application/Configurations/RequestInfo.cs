using vaccine.Domain.Enums;

namespace vaccine.Application.Configurations;

public class RequestInfo : IRequestInfo
{
    public Guid? UserId { get; set;  } = Guid.NewGuid();

    public string? Name { get; set; } = null!;

    public string? EmailAddress { get; set;  } = null!;
    
    public ERole? Role { get; set;  } = null!;

    public string? IP { get; set;  }

    public string? Port { get; set;  }

    public Guid CorrelationId { get; set; }

    public void SetUserInfo(
        string? userId,
        string? name,
        string? email,
        string role)
    {
        Name = name;
        EmailAddress = email;
        Guid.TryParse(userId, out var user);
        UserId = user;
        
        if (!string.IsNullOrWhiteSpace(role) &&
            int.TryParse(role, out var roleValue))
        {
            Role = (ERole)roleValue;
        }
        else
        {
            Role = null;
        }
    }

    public void SetIP(string ip)
    {
        IP = ip;
    }

    public void SetPort(string port)
    {
        Port = port;
    }

    public void SetCorrelationId(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
}