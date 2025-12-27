using vaccine.Data.Entities;
using vaccine.Endpoints.DTOs.Validators;

namespace vaccine.Domain.Entities;

public class Person : BaseEntity
{
    public Person() : base() { }

    public Person(string name, Cpf document, DateTime birthDate) : base()
    {
        Name = name;
        Document = document;
        Birthday = birthDate;
    }
    
    public string Name { get; set; }
    public Guid? UserId { get; set; }
    public Cpf Document { get; set; }
    public DateTime Birthday { get; set; }
    public User? User { get; set; }
    public List<Vaccination> Vaccinations { get; set; } = new();
}