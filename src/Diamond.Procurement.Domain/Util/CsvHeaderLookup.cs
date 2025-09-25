using System;

namespace Diamond.Procurement.Domain.Util;

public static class CsvHeaderLookup
{
    public static string[] Normalize(string[]? headers)
    {
        if (headers is null || headers.Length == 0)
            return Array.Empty<string>();

        var normalized = new string[headers.Length];
        for (var i = 0; i < headers.Length; i++)
            normalized[i] = headers[i] ?? string.Empty;
        return normalized;
    }

    public static string FindContains(string[] headers, params string[] candidates)
    {
        if (candidates is null || candidates.Length == 0)
            throw new ArgumentException("At least one candidate header must be provided.", nameof(candidates));

        foreach (var header in headers)
        {
            var hs = (header ?? string.Empty).Trim();
            foreach (var candidate in candidates)
            {
                if (hs.Contains(candidate, StringComparison.OrdinalIgnoreCase))
                    return header ?? string.Empty;
            }
        }

        throw new InvalidOperationException($"Could not find header: {string.Join(", ", candidates)}");
    }

    public static int IndexOf(string[] headers, string target, bool allowLeadingSpace = false)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            var header = headers[i] ?? string.Empty;
            var hs = allowLeadingSpace ? header.TrimStart() : header.Trim();
            if (hs.Equals(target, StringComparison.OrdinalIgnoreCase))
                return i;
        }

        throw new InvalidOperationException($"Could not find column header '{target}'.");
    }
}
