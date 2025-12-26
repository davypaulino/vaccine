using vaccine.Domain.Entities;
using vaccine.Domain.Enums;

namespace vaccine.Data.Entities;

public class User : BaseEntity
{
    public Guid? PersonId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public ERole Role { get; set; } = ERole.Person;
    public EStatus Status { get; set; } = EStatus.Active;
}