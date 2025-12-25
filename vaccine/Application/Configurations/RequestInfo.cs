namespace vaccine.Application.Configurations;

public class RequestInfo : IRequestInfo
{
    public Guid? UserId { get; set;  }

    public string? Name { get; set;  }

    public string? EmailAddress { get; set;  }

    public string? IP { get; set;  }

    public string? Port { get; set;  }

    public Guid CorrelationId { get; set; }

    public void SetUserInfo(
        Guid? userId,
        string? name,
        string? email)
    {
        
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