using System.Globalization;
using Taskmony.Errors;
using Taskmony.Exceptions;

namespace Taskmony.Services;

public class TimeConverter : ITimeConverter
{
    private const string Format = "yyyy-MM-ddTHH:mm:ss.fffZ";

    public DateTime StringToDateTimeUtc(string rfc3339)
    {
        if (!DateTime.TryParse(rfc3339, CultureInfo.InvariantCulture, out var dateTime))
        {
            throw new DomainException(ValidationErrors.InvalidDateTimeFormat);
        }

        return dateTime.ToUniversalTime();
    }

    public string DateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString(Format, CultureInfo.InvariantCulture);
    }
}