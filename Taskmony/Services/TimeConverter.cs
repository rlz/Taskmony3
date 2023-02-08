using System.Globalization;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Services.Abstract;

namespace Taskmony.Services;

public class TimeConverter : ITimeConverter
{
    private const string Format = "yyyy-MM-ddTHH:mm:ss.fffZ";

    public DateTime StringToDateTimeUtc(string dateTimeUtc)
    {
        if (!DateTime.TryParse(dateTimeUtc, CultureInfo.InvariantCulture, out var dateTime))
        {
            throw new DomainException(ValidationErrors.InvalidDateTimeFormat);
        }

        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    public string DateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString(Format, CultureInfo.InvariantCulture);
    }
}