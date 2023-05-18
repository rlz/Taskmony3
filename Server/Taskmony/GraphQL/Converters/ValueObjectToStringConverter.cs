using System.Diagnostics.CodeAnalysis;
using HotChocolate.Utilities;
using Taskmony.Models.ValueObjects;
using Taskmony.Services.Abstract;
using ValueOf;

namespace Taskmony.GraphQL.Converters;

public class ValueObjectToStringConverter : IChangeTypeProvider
{
    private readonly ITimeConverter _timeConverter;

    public ValueObjectToStringConverter(ITimeConverter timeConverter)
    {
        _timeConverter = timeConverter;
    }

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
                    ValueOf<DateTime, DeletedAt> deletedAt => _timeConverter.DateTimeToString(deletedAt.Value),
                    ValueOf<DateTime, CompletedAt> completedAt => _timeConverter.DateTimeToString(completedAt.Value),
                    ValueOf<string, Description> description => description.ToString(),
                    ValueOf<string, CommentText> commentText => commentText.ToString(),
                    ValueOf<string, DirectionName> directionName => directionName.ToString(),
                    ValueOf<string, DisplayName> displayName => displayName.ToString(),
                    ValueOf<string, Email> email => email.ToString(),
                    ValueOf<string, Login> login => login.ToString(),
                    ValueOf<DateTime, ReviewedAt> reviewedAt => _timeConverter.DateTimeToString(reviewedAt.Value),
                    ValueOf<string?, Details> details => details.ToString(),
                    _ => input
                };
            };

            return true;
        }

        converter = null;
        return false;
    }
}