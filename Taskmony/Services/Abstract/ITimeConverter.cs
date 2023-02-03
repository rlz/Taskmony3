namespace Taskmony.Services.Abstract;

public interface ITimeConverter
{
    DateTime StringToDateTimeUtc(string dateTimeUtc);

    string DateTimeToString(DateTime dateTime);
}