using ClosedXML.Excel;

namespace Diamond.Procurement.Domain.Util;

public static class XlExtensions
{
    public static double GetDoubleOrDefault(this IXLCell c)
    {
        if (!c.IsEmpty() && c.DataType == XLDataType.Number)
            return c.GetDouble();

        var s = c.GetString();
        if (double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d))
            return d;

        if (double.TryParse(s, out d))
            return d;

        return 0d;
    }
}
