using System.Text;

namespace vaccine.Endpoints.DTOs.Validators;

    
/// <summary>
/// Cpf value object
/// </summary>
public class Cpf : ValueObject
{
    public const int LENGTH_CPF = 11;

    public Cpf(string number)
    {
        Number = number.RemoveNonDigits();
        Number = Number.Substring(Math.Max(0, Number.Length - LENGTH_CPF));
    }

    public string Number { get; private set; }

    public override bool IsValid()
    {
        if (!IsNumeric(Number) || Number.Length != LENGTH_CPF || IsCpfDigitsEqual(Number))
        {
            return false;
        }

        var digits = Number[..9];
        var firstDigit = CalculateCpfDigit(digits, new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 });
        var secondDigit = CalculateCpfDigit(digits + firstDigit, new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 });

        return Number.EndsWith(firstDigit + secondDigit);
    }

    public override string ToString()
    {
        return Number;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }

    private static bool IsNumeric(string value)
    {
        return long.TryParse(value, out _);
    }

    private static bool IsCpfDigitsEqual(string value)
    {
        var firstDigit = value[0];
        return value.All(digit => digit == firstDigit);
    }

    private static string CalculateCpfDigit(string digits, int[] multipliers)
    {
        var sum = 0;
        for (int i = 0; i < digits.Length; i++)
        {
            sum += (digits[i] - '0') * multipliers[i];
        }
        var remainder = sum % LENGTH_CPF;
        return remainder < 2 ? "0" : (LENGTH_CPF - remainder).ToString();
    }
}

/// <summary>
/// Base value object 
/// </summary>
public abstract class ValueObject
{
    public abstract bool IsValid();

    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }
        return left is null || left.Equals(right!);
    }

    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !EqualOperator(left, right);
    }

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x is not null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public ValueObject GetCopy()
    {
        return (MemberwiseClone() as ValueObject)!;
    }
}


public static class StringExtensions
{
    public static string RemoveNonDigits(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        StringBuilder stringBuilder = new StringBuilder(text.Length);
        if (!string.IsNullOrEmpty(text))
        {
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                    stringBuilder.Append(c);
            }
        }
        return stringBuilder.ToString();
    }
}