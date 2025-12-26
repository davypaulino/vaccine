using System.Text;

namespace vaccine.Application.Configurations;

public class AuthenticationSettings
{
    public string SecretKeyValue { get; set; }
    public byte[] SecretKey => Encoding.ASCII.GetBytes(SecretKeyValue);
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpiryMinutes { get; set; }
    public string AdminName { get; set; } = null!;
    public string AdminPassword { get; set; } = null!;
}