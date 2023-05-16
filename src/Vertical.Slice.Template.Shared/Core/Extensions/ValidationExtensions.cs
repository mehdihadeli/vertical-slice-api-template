using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Vertical.Slice.Template.Shared.Core.Extensions;

// https://dev.to/lambdasharp/c-asserting-a-value-is-not-null-in-null-aware-code-f8m
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/nullable-analysis
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/attributes/caller-information
public static class ValidationExtensions
{
    private static readonly HashSet<string> _allowedCurrency = new() { "USD", "EUR", };

    public static T NotBeNull<T>(
        [NotNull] this T? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument == null)
        {
            throw new ArgumentNullException(
                message: $"{argumentName} cannot be null or empty.",
                paramName: argumentName
            );
        }

        return argument;
    }

    public static T NotBeNull<T>([NotNull] this T? argument, Exception exception)
    {
        if (argument == null)
        {
            throw exception;
        }

        return argument;
    }

    public static string NotBeEmpty(
        this string argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument.Length == 0)
        {
            throw new ArgumentException($"{argumentName} cannot be null or empty.", argumentName);
        }

        return argument;
    }

    public static string NotBeEmptyOrNull(
        [NotNull] this string? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (string.IsNullOrEmpty(argument))
        {
            throw new ArgumentException($"{argumentName} cannot be null or empty.", argumentName);
        }

        return argument;
    }

    public static string NotBeNullOrWhiteSpace(
        [NotNull] this string? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            throw new ArgumentException($"{argumentName} cannot be null or white space.", argumentName);
        }

        return argument;
    }

    public static Guid NotBeEmpty(
        this Guid argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument == Guid.Empty)
        {
            throw new ArgumentException($"{argumentName} cannot be empty.", argumentName);
        }

        return argument;
    }

    public static Guid NotBeEmpty(
        [NotNull] this Guid? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument is null)
        {
            throw new ArgumentNullException(
                message: $"{argumentName} cannot be null or empty.",
                paramName: argumentName
            );
        }

        return argument.Value.NotBeEmpty();
    }

    public static int NotBeNegativeOrZero(
        this int argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument == 0)
        {
            throw new ArgumentException($"{argumentName} cannot be zero.", argumentName);
        }

        return argument;
    }

    public static long NotBeNegativeOrZero(
        [NotNull] this long? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument is null)
        {
            throw new ArgumentNullException(
                message: $"{argumentName} cannot be null or empty.",
                paramName: argumentName
            );
        }

        return argument.Value.NotBeNegativeOrZero();
    }

    public static long NotBeNegativeOrZero(
        this long argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument == 0)
        {
            throw new ArgumentException($"{argumentName} cannot be zero.", argumentName);
        }

        return argument;
    }

    public static long NotBeNegativeOrZero(
        [NotNull] this int? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument is null)
        {
            throw new ArgumentNullException(
                message: $"{argumentName} cannot be null or empty.",
                paramName: argumentName
            );
        }

        return argument.Value.NotBeNegativeOrZero();
    }

    public static decimal NotBeNegativeOrZero(
        this decimal argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument == 0)
        {
            throw new ArgumentException($"{argumentName} cannot be zero.", argumentName);
        }

        return argument;
    }

    public static decimal NotBeNegativeOrZero(
        [NotNull] this decimal? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument is null)
        {
            throw new ArgumentNullException(
                message: $"{argumentName} cannot be null or empty.",
                paramName: argumentName
            );
        }

        return argument.Value.NotBeNegativeOrZero();
    }

    public static double NotBeNegativeOrZero(
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

    public static double NotBeNegativeOrZero(
        [NotNull] this double? argument,
        [CallerArgumentExpression("argument")] string? argumentName = null
    )
    {
        if (argument is null)
        {
            throw new ArgumentNullException(
                message: $"{argumentName} cannot be null or empty.",
                paramName: argumentName
            );
        }

        return argument.Value.NotBeNegativeOrZero();
    }

    public static string NotBeInvalidEmail(
        this string email,
        [CallerArgumentExpression("email")] string? argumentName = null
    )
    {
        // Use Regex to validate email format
        var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        if (!regex.IsMatch(email))
        {
            throw new ArgumentException($"{argumentName} is not a valid email address.", argumentName);
        }

        return email;
    }

    public static string NotBeInvalidPhoneNumber(
        this string phoneNumber,
        [CallerArgumentExpression("phoneNumber")] string? argumentName = null
    )
    {
        // Use Regex to validate phone number format
        var regex = new Regex(@"^[+]?(\d{1,2})?[\s.-]?(\d{3})[\s.-]?(\d{4})[\s.-]?(\d{4})$");
        if (!regex.IsMatch(phoneNumber))
        {
            throw new ArgumentException($"{argumentName} is not a valid phone number.", argumentName);
        }

        return phoneNumber;
    }

    public static string NotBeInvalidMobileNumber(
        this string mobileNumber,
        [CallerArgumentExpression("mobileNumber")] string? argumentName = null
    )
    {
        // Use Regex to validate mobile number format
        var regex = new Regex(@"^(?:(?:\+|00)([1-9]{1,3}))?([1-9]\d{9})$");
        if (!regex.IsMatch(mobileNumber))
        {
            throw new ArgumentException($"{argumentName} is not a valid mobile number.", argumentName);
        }

        return mobileNumber;
    }

    public static string NotBeInvalidCurrency(
        this string currency,
        [CallerArgumentExpression("currency")] string? argumentName = null
    )
    {
        currency = currency.ToUpperInvariant();
        if (!_allowedCurrency.Contains(currency))
        {
            throw new ArgumentException($"{argumentName} is not a valid currency.", argumentName);
        }

        return currency;
    }

    public static TEnum NotBeEmptyOrNull<TEnum>(
        [NotNull] this TEnum? enumValue,
        [CallerArgumentExpression("enumValue")] string argumentName = ""
    )
        where TEnum : Enum
    {
        if (enumValue is null)
        {
            throw new ArgumentNullException(
                message: $"{argumentName} cannot be null or empty.",
                paramName: argumentName
            );
        }

        enumValue.NotBeEmpty();

        return enumValue;
    }

    public static TEnum NotBeEmpty<TEnum>(
        [NotNull] this TEnum enumValue,
        [CallerArgumentExpression("enumValue")] string? argumentName = null
    )
        where TEnum : Enum
    {
        enumValue.NotBeNull();
        if (enumValue.Equals(default(TEnum)))
        {
            throw new ArgumentException(
                $"The value of '{argumentName}' cannot be the default value of '{typeof(TEnum).Name}' enum."
            );
        }

        return enumValue;
    }
}
