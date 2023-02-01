using System.Diagnostics.CodeAnalysis;
using HotChocolate.Utilities;
using Taskmony.ValueObjects;
using ValueOf;

namespace Taskmony.GraphQL.Converters;

public class ValueObjectToStringConverter : IChangeTypeProvider
{
    public bool TryCreateConverter(Type source, Type target, ChangeTypeProvider root,
        [NotNullWhen(true)] out ChangeType? converter)
    {
        if (source.BaseType is { IsGenericType: true } &&
            source.BaseType.GetGenericTypeDefinition() == typeof(ValueOf<,>) && target == typeof(string))
        {
            converter = input =>
            {
                return input switch
                {
                    ValueOf<DateTime, DeletedAt> deletedAt => deletedAt.ToString(),
                    ValueOf<DateTime, CompletedAt> completedAt => completedAt.ToString(),
                    ValueOf<DateTime, StartAt> startAt => startAt.ToString(),
                    ValueOf<string, Description> description => description.ToString(),
                    ValueOf<string, CommentText> commentText => commentText.ToString(),
                    ValueOf<string, DirectionName> directionName => directionName.ToString(),
                    ValueOf<string, DisplayName> displayName => displayName.ToString(),
                    ValueOf<string, Email> email => email.ToString(),
                    ValueOf<string, Login> login => login.ToString(),
                    ValueOf<DateTime, ReviewedAt> reviewedAt => reviewedAt.ToString(),
                    _ => input
                };
            };

            return true;
        }

        converter = null;
        return false;
    }
}