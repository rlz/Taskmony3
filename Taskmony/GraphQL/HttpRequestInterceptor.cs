using System.Security.Claims;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;

namespace Taskmony.GraphQL;

public class HttpRequestInterceptor : DefaultHttpRequestInterceptor
{
    public override ValueTask OnCreateAsync(HttpContext context, IRequestExecutor requestExecutor,
        IQueryRequestBuilder requestBuilder, CancellationToken cancellationToken)
    {
        if (context.User.Identity is not null && context.User.Identity.IsAuthenticated)
        {
            var userId = Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                                    ?? throw new ArgumentException("User identifier claim is required",
                                        nameof(context)));

            requestBuilder.SetGlobalState("currentUserId", userId);
        }

        return base.OnCreateAsync(context, requestExecutor, requestBuilder,
            cancellationToken);
    }
}