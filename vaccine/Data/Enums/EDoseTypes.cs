using System.ComponentModel;
using System.Reflection;

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
    [Description("Nenhum dose")]
    None = 0,

    /// <summary>
    /// First dose of the vaccine.
    /// Value = 1
    /// </summary>
    [Description("Primeira dose")]
    First = 1 << 0,

    /// <summary>
    /// Second dose of the vaccine.
    /// Value = 2
    /// </summary>
    [Description("Segunda dose")]
    Second = 1 << 1,

    /// <summary>
    /// Third dose of the vaccine (when applicable).
    /// Value = 4
    /// </summary>
    [Description("Terceira dose")]
    Third = 1 << 2,

    /// <summary>
    /// First booster (reinforcement) dose.
    /// Value = 8
    /// </summary>
    [Description("Primeira dose (reforço)")]
    FirstReinforcement = 1 << 3,

    /// <summary>
    /// Second booster (reinforcement) dose.
    /// Value = 16
    /// </summary>
    [Description("Segunda dose (reforço)")]
    SecondReinforcement = 1 << 4
}

public static class EnumDescriptionHelper
{
    public static string GetEnumDescription<T>()
        where T : struct, Enum
    {
        return string.Join($"{Environment.NewLine}",
            Enum.GetValues<T>().Select(e =>
            {
                var member = typeof(T)
                    .GetMember(e.ToString())
                    .First();

                var description = member
                    .GetCustomAttribute<DescriptionAttribute>()?
                    .Description;

                return description is null
                    ? $"- {e} ({Convert.ToInt32(e)})"
                    : $"- {e} ({Convert.ToInt32(e)}): {description}";
            }));
    }
}