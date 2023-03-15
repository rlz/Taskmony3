using System.Diagnostics.CodeAnalysis;
using HotChocolate.Utilities;

namespace Taskmony.GraphQL.Converters;

public class StringToGuidConverter : IChangeTypeProvider
{
    public bool TryCreateConverter(Type source, Type target, ChangeTypeProvider root,
        [NotNullWhen(true)] out ChangeType? converter)
    {
        if (source == typeof(string) && target == typeof(Guid))
        {
            converter = input =>
            {
                var stringId = input?.ToString();

                if (stringId is not null)
                {
                    if (!Guid.TryParse(stringId, out var guid))
                        return Guid.Empty;

                    return guid;
                }

                return null;
            };

            return true;
        }

        converter = null;
        return false;
    }
}