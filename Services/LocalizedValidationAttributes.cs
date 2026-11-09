using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Bayan.Services
{
    public class LocalizedRequiredAttribute : RequiredAttribute
    {
        public LocalizedRequiredAttribute(string key = "val.required")
        {
            ErrorMessage = AppStrings.Get(key);
        }
    }

    public class LocalizedEmailAttribute : ValidationAttribute
    {
        private static readonly EmailAddressAttribute Inner = new();

        public LocalizedEmailAttribute(string key = "val.email")
        {
            ErrorMessage = AppStrings.Get(key);
        }

        public override bool IsValid(object? value)
        {
            if (value is null || string.IsNullOrWhiteSpace(value as string))
                return true;

            return Inner.IsValid(value);
        }
    }

    public class LocalizedMinLengthAttribute : MinLengthAttribute
    {
        public LocalizedMinLengthAttribute(int length, string key = "val.minLength") : base(length)
        {
            ErrorMessage = AppStrings.Format(key, length, length);
        }
    }

    public class LocalizedMaxLengthAttribute : MaxLengthAttribute
    {
        public LocalizedMaxLengthAttribute(int length, string key = "val.maxLength") : base(length)
        {
            ErrorMessage = AppStrings.Format(key, length, length);
        }
    }

    public class LocalizedCompareAttribute : CompareAttribute
    {
        public LocalizedCompareAttribute(string otherProperty, string key = "val.compare") : base(otherProperty)
        {
            ErrorMessage = AppStrings.Get(key);
        }
    }

    public class LocalizedRegularExpressionAttribute : RegularExpressionAttribute
    {
        public LocalizedRegularExpressionAttribute(string pattern, string key) : base(pattern)
        {
            ErrorMessage = AppStrings.Get(key);
        }
    }

    public class LocalizedRangeAttribute : RangeAttribute
    {
        public LocalizedRangeAttribute(double minimum, double maximum, string key) : base(minimum, maximum)
        {
            ErrorMessage = AppStrings.Get(key);
        }

        public LocalizedRangeAttribute(int minimum, int maximum, string key) : base(minimum, maximum)
        {
            ErrorMessage = AppStrings.Get(key);
        }
    }

    public static class CultureConfig
    {
        public static readonly string[] SupportedCultures = { "en", "ar" };
        public const string DefaultCulture = "en";

        public static CultureInfo Resolve(string? culture)
        {
            var name = SupportedCultures.Contains(culture, StringComparer.OrdinalIgnoreCase)
                ? culture!
                : DefaultCulture;

            return CultureInfo.GetCultureInfo(name);
        }
    }
}
