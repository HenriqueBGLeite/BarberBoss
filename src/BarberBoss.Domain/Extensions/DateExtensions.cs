using BarberBoss.Domain.Reports;
using System.Globalization;

namespace BarberBoss.Domain.Extensions;

public static class DateExtensions
{
    public static string ToLongDateNoDayOfWeek(this DateTime date)
    {
        var currentCulture = CultureInfo.CurrentCulture;

        if (currentCulture.Name == "en")
            return date.ToString($"MMMM dd, yyyy");

        return date.ToString($"dd '{ResourceReportGenerationMessages.OF}' MMMM '{ResourceReportGenerationMessages.OF}' yyyy");
    }
}
