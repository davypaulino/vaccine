namespace vaccine.Data.Enums;

/// <summary>
/// Represents the types of vaccine doses applied to a person.
/// This enum uses bitwise flags to allow combination and validation
/// of multiple dose types taken or required.
/// </summary>
[Flags]
public enum EDoseType
{
    /// <summary>
    /// No dose applied.
    /// </summary>
    None = 0,

    /// <summary>
    /// First dose of the vaccine.
    /// Value =  1
    /// </summary>
    First = 1 << 0,

    /// <summary>
    /// Second dose of the vaccine.
    /// Value = 2
    /// </summary>
    Second = 1 << 1,

    /// <summary>
    /// Third dose of the vaccine (when applicable).
    /// Value = 4
    /// </summary>
    Third = 1 << 2,

    /// <summary>
    /// First booster (reinforcement) dose.
    /// Value = 8
    /// </summary>
    FirstReinforcement = 1 << 3,

    /// <summary>
    /// Second booster (reinforcement) dose.
    /// Value = 16
    /// </summary>
    SecondReinforcement = 1 << 4
}

