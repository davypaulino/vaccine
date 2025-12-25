using Microsoft.EntityFrameworkCore;
using vaccine.Data.Enums;

namespace vaccine.Data.Entities;

public class Vaccine
{
    public Guid Id { get; set; } = Guid.NewGuid();
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

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

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
}