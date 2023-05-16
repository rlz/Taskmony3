using System.Diagnostics.CodeAnalysis;
using HotChocolate.Utilities;
using Taskmony.Models.Tasks;

namespace Taskmony.GraphQL.Converters;

public class WeekDaysToWeekDayConverter : IChangeTypeProvider
{
    public bool TryCreateConverter(Type source, Type target, ChangeTypeProvider root,
        [NotNullWhen(true)] out ChangeType? converter)
    {
        if (typeof(IEnumerable<WeekDay>).IsAssignableFrom(source) && target == typeof(WeekDay?))
        {
            converter = input =>
            {
                if (input is IEnumerable<WeekDay> weekDays)
                {
                    var enumerable = weekDays as WeekDay[] ?? weekDays.ToArray();
                    
                    if (!enumerable.Any())
                        return null;

                    return enumerable.Aggregate((current, weekDay) => current | weekDay);
                }

                return null;
            };

            return true;
        }

        converter = null;
        return false;
    }
}