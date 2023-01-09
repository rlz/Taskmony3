using System.Globalization;
using System.Xml;

namespace Taskmony.Services;

public class TimeConverter : ITimeConverter
{
    private const string Format = "yyyy-MM-ddTHH:mm:ss.fffZ";

    public DateTime Rfc3339ToDateTimeUtc(string rfc3339)
    {
        return XmlConvert.ToDateTime(rfc3339, XmlDateTimeSerializationMode.Utc);
    }

    public string DateTimeToRfc3339(DateTime dateTime)
    {
        return dateTime.ToString(Format, CultureInfo.InvariantCulture);
    }
}