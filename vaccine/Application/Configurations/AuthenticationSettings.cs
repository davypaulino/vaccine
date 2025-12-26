using System.ComponentModel.DataAnnotations;
using System.Text;

namespace vaccine.Application.Configurations;

public class AuthenticationSettings
{
    [Required]
    [MinLength(32, ErrorMessage = "SecretKeyValue must be at least 32 characters long.")]
    public string SecretKeyValue { get; set; } = null!;

    public byte[] SecretKey => Encoding.ASCII.GetBytes(SecretKeyValue);

    [Required]
    public string Issuer { get; set; } = null!;

    [Required]
    public string Audience { get; set; } = null!;

    [Range(1, 1440, ErrorMessage = "ExpiryMinutes must be between 1 and 1440.")]
    public int ExpiryMinutes { get; set; }

    [Required]
    public string AdminName { get; set; } = null!;

    [Required]
    [MinLength(8, ErrorMessage = "AdminPassword must be at least 8 characters long.")]
    public string AdminPassword { get; set; } = null!;
}