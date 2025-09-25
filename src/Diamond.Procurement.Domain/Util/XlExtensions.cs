using System.Globalization;
using ClosedXML.Excel;

namespace Diamond.Procurement.Domain.Util;

public static class XlExtensions
{
    public static double GetDoubleOrDefault(this IXLCell c)
    {
        if (!c.IsEmpty() && c.DataType == XLDataType.Number)
            return c.GetDouble();

        var s = c.GetString();
        if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            return d;

        if (double.TryParse(s, out d))
            return d;

        return 0d;
    }

    public static int GetIntOrDefault(this IXLCell cell)
    {
        if (cell.DataType == XLDataType.Number)
            return (int)Math.Round(cell.GetDouble());

        var text = cell.GetString();
        if (NumericParsers.TryParseSignedInt(text, out var value))
            return value;

        if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
            return (int)Math.Round(d);

        return 0;
    }
}
