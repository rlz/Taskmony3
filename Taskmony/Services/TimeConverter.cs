using System.Globalization;

namespace Taskmony.Services;

public class TimeConverter : ITimeConverter
{
    private const string Format = "yyyy-MM-ddTHH:mm:ss.fffZ";

    public DateTime StringToDateTimeUtc(string rfc3339)
    {
        return DateTime.Parse(rfc3339, CultureInfo.InvariantCulture).ToUniversalTime();
    }

    public string DateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString(Format, CultureInfo.InvariantCulture);
    }
}