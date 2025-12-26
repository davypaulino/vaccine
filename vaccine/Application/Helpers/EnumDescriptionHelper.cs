using System.ComponentModel;
using System.Reflection;

namespace vaccine.Application.Helpers;

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