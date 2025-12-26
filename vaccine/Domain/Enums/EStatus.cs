using System.ComponentModel;

namespace vaccine.Domain.Enums;

/// <summary>
/// Represents the current status of an entity in the system,
/// such as a user account, registration process, or workflow item.
/// </summary>
public enum EStatus
{
    /// <summary>
    /// Entity is pending approval or activation.
    /// Value = 0
    /// </summary>
    [Description("Pendente")]
    Pending = 0,

    /// <summary>
    /// Entity is active and fully operational.
    /// Value = 1
    /// </summary>
    [Description("Ativo")]
    Active = 1,

    /// <summary>
    /// Entity is inactive or disabled.
    /// No operations should be performed while in this state.
    /// Value = 2
    /// </summary>
    [Description("Inativo")]
    Inactive = 2
}