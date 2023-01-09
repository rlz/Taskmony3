namespace Taskmony.Services;

public interface ITimeConverter
{
    DateTime Rfc3339ToDateTimeUtc(string rfc3339);

    string DateTimeToRfc3339(DateTime dateTime);
}