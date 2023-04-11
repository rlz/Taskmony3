using System.Diagnostics.CodeAnalysis;
using HotChocolate.Utilities;
using Taskmony.Models.Enums;

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
                var weekDays = input as IEnumerable<WeekDay>;

                if (weekDays is not null)
                {
                    if (weekDays.Count() == 0)
                        return null;

                    return weekDays.Aggregate((current, weekDay) => current | weekDay);
                }

                return null;
            };

            return true;
        }

        converter = null;
        return false;
    }
}