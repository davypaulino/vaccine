using System.ComponentModel;

namespace vaccine.Domain.Enums;

/// <summary>
/// Represents the roles assigned to a user within the system.
/// This enum uses bitwise flags to allow combination and validation
/// of multiple user roles.
/// </summary>
[Flags]
public enum ERole
{
    /// <summary>
    /// Standard system user.
    /// Typically represents a vaccinated person or basic user.
    /// Value = 0
    /// </summary>
    [Description("Pessoa")]
    Person = 0,

    /// <summary>
    /// User with read-only access.
    /// Can view information but cannot modify data.
    /// Value = 1
    /// </summary>
    [Description("Visualizador")]
    Viewer = 1 << 0,

    /// <summary>
    /// User with edit permissions.
    /// Can create or update records but has limited administrative access.
    /// Value = 2
    /// </summary>
    [Description("Editor")]
    Editor = 1 << 1,

    /// <summary>
    /// System administrator.
    /// Has full access to all system features and settings.
    /// Value = 4
    /// </summary>
    [Description("Administrador")]
    Admin = 1 << 2
}