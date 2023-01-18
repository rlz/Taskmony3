namespace Taskmony.Services;

public interface ITimeConverter
{
    DateTime StringToDateTimeUtc(string rfc3339);

    string DateTimeToString(DateTime dateTime);
}