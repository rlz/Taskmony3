namespace Taskmony.GraphQL;

public class QueryType : ObjectType<Query>
{
    protected override void Configure(
        IObjectTypeDescriptor<Query> descriptor)
    {
        //descriptor.Authorize();
    }
}