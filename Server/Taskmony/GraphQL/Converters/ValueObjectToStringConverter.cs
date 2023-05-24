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
                    ValueOf<string, Description> description => description.Value,
                    ValueOf<string, CommentText> commentText => commentText.Value,
                    ValueOf<string, DirectionName> directionName => directionName.Value,
                    ValueOf<string, DisplayName> displayName => displayName.Value,
                    ValueOf<string, Email> email => email.Value,
                    ValueOf<string, Login> login => login.Value,
                    ValueOf<DateTime, ReviewedAt> reviewedAt => _timeConverter.DateTimeToString(reviewedAt.Value),
                    ValueOf<string?, Details> details => details?.Value,
                    _ => input
                };
            };

            return true;
        }

        converter = null;
        return false;
    }
}