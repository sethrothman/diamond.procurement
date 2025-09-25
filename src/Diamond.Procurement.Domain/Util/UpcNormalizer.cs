namespace Diamond.Procurement.Domain.Util;

public static class UpcNormalizer
{
    public static bool TryNormalizeTo10(string? raw, out string upc10, out string? reason)
    {
        reason = null;
        upc10 = string.Empty;
        if (string.IsNullOrWhiteSpace(raw)) { reason = "empty"; return false; }
        var s = raw.Trim().ToUpperInvariant();
        if (s.EndsWith("EA")) s = s[..^2];
        s = s.Replace("-", "").Replace(" ", "");

        //if (s.StartsWith("0")) s = s[1..];

        if (s.Length == 0) { reason = "empty after cleaning"; return false; }

        foreach (var ch in s)
            if (ch < '0' || ch > '9') { reason = "non-digit"; return false; }

        if (s.Length > 10) s = s[^10..];
        if (s.Length < 10) { reason = "len<10"; return false; }

        upc10 = s;
        return true;
    }
}
