using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Diamond.Procurement.Domain.Util;

public static class FileNameDateParser
{
    public static DateTime? ExtractDateFromFileName(string filePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);

        // Regex patterns for common date formats
        string[] patterns =
        {
            @"\b\d{4}[-]?\d{2}[-]?\d{2}\b",     // 20250801 or 2025-08-01
            @"\b\d{1,2}[-/]\d{1,2}[-/]\d{2,4}\b" // 8-1-25 or 08/01/2025
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(fileName, pattern);
            if (match.Success)
            {
                string candidate = match.Value;

                // List of accepted formats
                string[] formats =
                {
                    "yyyyMMdd",
                    "yyyy-MM-dd",
                    "MM-dd-yy",
                    "M-d-yy",
                    "MM-dd-yyyy",
                    "M-d-yyyy",
                    "MM/dd/yy",
                    "M/d/yy",
                    "MM/dd/yyyy",
                    "M/d/yyyy"
                };

                if (DateTime.TryParseExact(candidate, formats,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsed))
                {
                    return parsed;
                }

                // As fallback, try general parse
                if (DateTime.TryParse(candidate, out parsed))
                {
                    return parsed;
                }
            }
        }

        return null; // no date found
    }
}
