using Taskmony.Models;

namespace Taskmony.GraphQL.Directions;

public class DirectionType : ObjectType<Direction>
{
    protected override void Configure(IObjectTypeDescriptor<Direction> descriptor)
    {
        descriptor.Field(d => d.CreatedById).Ignore();

        descriptor.Field(d => d.Name).Type<StringType>();
        descriptor.Field(d => d.CreatedBy).Type<ObjectType<User>>();
        descriptor.Field(d => d.CreatedAt).Type<StringType>();
        descriptor.Field(d => d.DeletedAt).Type<StringType>();
    }
}