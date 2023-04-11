using Taskmony.Models;

namespace Taskmony.GraphQL.Users;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Field(u => u.AssignedBy).Ignore();
        descriptor.Field(u => u.AssignedTo).Ignore();
        descriptor.Field(u => u.CreatedAt).Ignore();
        descriptor.Field(u => u.OwnDirections).Ignore();
        descriptor.Field(u => u.Subscriptions).Ignore();
        descriptor.Field(u => u.Tasks).Ignore();
        descriptor.Field(u => u.Ideas).Ignore();
        descriptor.Field(u => u.Directions).Ignore();
        descriptor.Field(u => u.ActionItemType).Ignore();
        descriptor.Field(u => u.Comments).Ignore();
        descriptor.Field(u => u.RefreshTokens).Ignore();

        descriptor.Field(u => u.Login).Type<StringType>();
        descriptor.Field(u => u.CreatedAt).Type<StringType>();
        descriptor.Field(u => u.Password).Type<StringType>();
        descriptor.Field(u => u.Email).Type<StringType>();
        descriptor.Field(u => u.DisplayName).Type<StringType>();
        descriptor.Field(u => u.NotificationReadTime).Type<StringType>();
    }
}