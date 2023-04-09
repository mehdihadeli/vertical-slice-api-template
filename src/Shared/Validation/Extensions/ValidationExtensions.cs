using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Shared.Validation.Extensions;

public static class ValidationExtensions
{
    public static T NotNull<T>(
        [NotNull] this T? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument == null)
        {
            throw new ArgumentNullException(argumentName);
        }

        return argument;
    }

    public static string NotEmpty(
        [NotNull] this string? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentException($"{argumentName} cannot be null or empty.", argumentName);
        }

        return argument;
    }

    public static Guid NotEmpty(
        [NotNull] this Guid? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument is null)
        {
            throw new ArgumentException($"{argumentName} cannot be null.", argumentName);
        }

        return argument.Value.NotEmpty();
    }

    public static Guid NotEmpty(this Guid argument, [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument == Guid.Empty)
        {
            throw new ArgumentException($"{argumentName} cannot be empty.", argumentName);
        }

        return argument;
    }

    public static int NotZero(this int argument, [CallerArgumentExpression("argument")] string? argumentName = null)
    {
        if (argument == 0)
        {
            throw new ArgumentException($"{argumentName} cannot be zero.", argumentName);
        }

        return argument;
    }

    public static int NotZero(
        [NotNull] this int? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument is null)
        {
            throw new ArgumentException($"{argumentName} cannot be null.", argumentName);
        }

        return argument.Value.NotZero();
    }

    public static double NotZero(
        this double argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument == 0)
        {
            throw new ArgumentException($"{argumentName} cannot be zero.", argumentName);
        }

        return argument;
    }

    public static double NotZero(
        [NotNull] this double? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument is null)
        {
            throw new ArgumentException($"{argumentName} cannot be null.", argumentName);
        }

        return argument.Value.NotZero();
    }

    public static void IsValidEmail(
        [NotNull] this string? email,
        [CallerArgumentExpression("email")] string? argumentName = null
    )
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException($"{argumentName} cannot be null, empty or whitespace.", argumentName);
        }

        // Use Regex to validate email format
        var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        if (!regex.IsMatch(email))
        {
            throw new ArgumentException($"{argumentName} is not a valid email address.", argumentName);
        }
    }

    public static void IsValidPhoneNumber(
        [NotNull] this string? phoneNumber,
        [CallerArgumentExpression("phoneNumber")] string? argumentName = null
    )
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException($"{argumentName} cannot be null, empty or whitespace.", argumentName);
        }

        // Use Regex to validate phone number format
        var regex = new Regex(@"^[+]?(\d{1,2})?[\s.-]?(\d{3})[\s.-]?(\d{4})[\s.-]?(\d{4})$");
        if (!regex.IsMatch(phoneNumber))
        {
            throw new ArgumentException($"{argumentName} is not a valid phone number.", argumentName);
        }
    }

    public static void IsValidMobileNumber(
        [NotNull] this string? mobileNumber,
        [CallerArgumentExpression("mobileNumber")] string? argumentName = null
    )
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
        {
            throw new ArgumentException($"{argumentName} cannot be null, empty or whitespace.", argumentName);
        }

        // Use Regex to validate mobile number format
        var regex = new Regex(@"^(?:(?:\+|00)([1-9]{1,3}))?([1-9]\d{9})$");
        if (!regex.IsMatch(mobileNumber))
        {
            throw new ArgumentException($"{argumentName} is not a valid mobile number.", argumentName);
        }
    }
}
