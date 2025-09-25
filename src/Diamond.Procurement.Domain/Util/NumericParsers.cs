using System.Globalization;

namespace Diamond.Procurement.Domain.Util;

public static class NumericParsers
{
    public static bool TryParseSignedInt(string? raw, out int value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        var trimmed = raw.Trim();

        if (TryParseCore(trimmed.Replace(",", string.Empty), out value))
            return true;

        if (TryParseCore(trimmed, out value))
            return true;

        if (int.TryParse(trimmed, out value))
            return true;

        return false;
    }

    public static int ParseSignedIntOrDefault(string? raw, int defaultValue = 0)
        => TryParseSignedInt(raw, out var value) ? value : defaultValue;

    public static int? ParseOptionalSignedInt(string? raw)
        => TryParseSignedInt(raw, out var value) ? value : null;

    public static decimal? ParseMoney(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var s = raw.Trim();
        var negative = false;

        if (s.EndsWith("-", StringComparison.Ordinal))
        {
            negative = true;
            s = s[..^1];
        }

        if (s.StartsWith("(", StringComparison.Ordinal) && s.EndsWith(")", StringComparison.Ordinal))
        {
            negative = true;
            s = s[1..^1];
        }

        s = s.Replace("$", string.Empty).Replace(",", string.Empty);

        if (decimal.TryParse(s, NumberStyles.Number | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value))
            return negative ? -value : value;

        return null;
    }

    private static bool TryParseCore(string input, out int value)
    {
        value = 0;
        if (string.IsNullOrEmpty(input))
            return false;

        var s = input;
        var negative = false;

        if (s.EndsWith("-", StringComparison.Ordinal))
        {
            negative = true;
            s = s[..^1];
        }

        if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            return false;

        value = negative ? -parsed : parsed;
        return true;
    }
}
