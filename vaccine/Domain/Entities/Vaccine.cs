using vaccine.Domain.Enums;

namespace vaccine.Domain.Entities;

public class Vaccine : BaseEntity
{
    public string Name { get; set; }

    /// <summary>
    /// Defines which dose types are applicable for this vaccine.
    /// This value is a bitwise combination of <see cref="EDoseType"/>
    /// and is used to validate whether a given dose can be applied.
    ///
    /// Example:
    /// - Hepatitis B: First | Second | Third
    /// - COVID-19: First | Second | FirstReinforcement | SecondReinforcement
    /// - Influenza: First
    ///
    /// This property does NOT represent applied doses.
    /// Applied doses must be stored as individual records in the DOSE entity.
    /// </summary>
    public EDoseType AvailableTypes { get; set; }

    public List<Vaccination> Vaccinations { get; set; } = new();

    public Vaccine()
    {}

    public Vaccine(string name, EDoseType[] availableDoses)
    {
        Name = name;
        foreach (var dose in availableDoses)
        {
            AvailableTypes |= dose;
        }
    }
    
    public void Update(string name, EDoseType[] availableDoses)
    {
        Name = name;
        AvailableTypes = EDoseType.None;
        UpdatedAt = DateTime.UtcNow;
        foreach (var dose in availableDoses)
        {
            AvailableTypes |= dose;
        }
    }

    public IEnumerable<EDoseType> AvailableDoses()
    {
        return Enum.GetValues<EDoseType>()
            .Where(v =>
                v != EDoseType.None &&
                (AvailableTypes & v) == v)
            .ToList();
    }
}