using System.Diagnostics.CodeAnalysis;
using HotChocolate.Utilities;

namespace Taskmony.GraphQL.Converters;

public class GuidToStringConverter : IChangeTypeProvider
{
    public bool TryCreateConverter(Type source, Type target, ChangeTypeProvider root,
        [NotNullWhen(true)] out ChangeType? converter)
    {
        if (source == typeof(Guid) && target == typeof(string))
        {
            converter = input =>
            {
                if (input is Guid guid)
                {
                    return guid.ToString();
                }

                return input;
            };

            return true;
        }

        converter = null;
        return false;
    }
}