using Taskmony.Repositories;
using Task = Taskmony.Models.Task;

namespace Taskmony.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IDirectionService _directionService;

    public TaskService(ITaskRepository taskRepository, IDirectionService directionService)
    {
        _taskRepository = taskRepository;
        _directionService = directionService;
    }

    public async Task<IEnumerable<Task>> GetTasksAsync(Guid[]? id, Guid?[]? directionId, int? offset,
        int? limit, Guid currentUserId)
    {
        List<Task> tasks;

        //If directionId is [null] return tasks created by the current user with direction id = null
        if (directionId is not null && directionId.Length == 1 && directionId.Contains(null))
        {
            tasks = (await _taskRepository.GetTasksAsync(id, directionId, offset, limit, currentUserId)).ToList();
        }
        else
        {
            var userDirectionIds = await _directionService.GetUserDirectionIds(currentUserId);
            var authorizedDirectionIds = userDirectionIds.Cast<Guid?>().Append(null);

            //If directionId is null return all tasks visible to the current user.
            //That includes tasks from all the directions where user is a member
            //(user is a member of his own directions)

            directionId = directionId == null
                ? authorizedDirectionIds.ToArray()
                : directionId.Intersect(authorizedDirectionIds).ToArray();

            tasks = (await _taskRepository.GetTasksAsync(id, directionId, offset, limit, currentUserId)).ToList();
        }

        tasks.ForEach(t1 =>
            t1.NumberOfRepetitions = t1.GroupId == null ? null : tasks.Count(t2 => t2.GroupId == t1.GroupId));

        return tasks;
    }
}