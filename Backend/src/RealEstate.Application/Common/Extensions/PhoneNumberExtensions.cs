using System.Text.RegularExpressions;

namespace RealEstate.Application.Common.Extensions;

public static class PhoneNumberExtensions
{
    public static string ToCanonicalE164(this string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return string.Empty;

        var cleaned = Regex.Replace(phoneNumber, @"[^\d+]", "");

        if (cleaned.StartsWith("00"))
        {
            cleaned = "+" + cleaned[2..];
        }

        if (!cleaned.StartsWith("+"))
        {
            cleaned = "+" + cleaned;
        }

        return cleaned;
    }
}