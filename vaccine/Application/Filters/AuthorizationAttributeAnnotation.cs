using vaccine.Domain.Enums;

namespace vaccine.Application.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public sealed class AuthorizationAttributeAnnotation : Attribute
{
    public ERole[] Roles { get; }

    public AuthorizationAttributeAnnotation(ERole[] roles)
    {
        Roles = roles;
    }
}