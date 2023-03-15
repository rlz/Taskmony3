using System.Diagnostics.CodeAnalysis;
using HotChocolate.Utilities;
using Taskmony.Services.Abstract;

namespace Taskmony.GraphQL.Converters;

public class DateTimeToStringConverter : IChangeTypeProvider
{
    private readonly ITimeConverter _timeConverter;
    
    public DateTimeToStringConverter(ITimeConverter timeConverter)
    {
        _timeConverter = timeConverter;
    }

    public bool TryCreateConverter(Type source, Type target, ChangeTypeProvider root,
        [NotNullWhen(true)] out ChangeType? converter)
    {
        if (source == typeof(DateTime) && target == typeof(string))
        {
            converter = input =>
            {
                if (input is DateTime dateTime)
                {
                    return _timeConverter.DateTimeToString(dateTime);
                }

                return input;
            };

            return true;
        }

        converter = null;
        return false;
    }
}