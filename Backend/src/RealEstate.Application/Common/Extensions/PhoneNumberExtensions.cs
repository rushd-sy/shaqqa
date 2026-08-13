using System.Text.RegularExpressions;

namespace RealEstate.Application.Common.Extensions;

public static class PhoneNumberExtensions
{
    public static string ToCanonicalE164(this string phoneNumber)
    {
        if(string.IsNullOrWhiteSpace(phoneNumber)) return string.Empty;

        var cleaned = phoneNumber.Trim().Replace(" ", "").Replace("-", "");

        if (cleaned.StartsWith("09"))
        {
            return "+963" + cleaned.Substring(1);
        }

        if (cleaned.StartsWith("00"))
        {
            return "+" + cleaned.Substring(2);
        }

        if (!cleaned.StartsWith("+"))
        {
            return "+" + cleaned;
        }

        return cleaned;
    }
}