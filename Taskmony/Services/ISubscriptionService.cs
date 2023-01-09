using Taskmony.Models;

namespace Taskmony.Services;

public interface ISubscriptionService
{
    Task<IEnumerable<User>> GetTaskSubscribersAsync(Guid taskId, Guid currentUserId);

    Task<IEnumerable<User>> GetIdeaSubscribersAsync(Guid ideaId, Guid currentUserId);
}