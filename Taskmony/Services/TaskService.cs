using Taskmony.Repositories;
using Task = Taskmony.Models.Task;

namespace Taskmony.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IDirectionRepository _directionRepository;

    public TaskService(ITaskRepository taskRepository, IDirectionRepository directionRepository)
    {
        _taskRepository = taskRepository;
        _directionRepository = directionRepository;
    }

    public async Task<IEnumerable<Task>> GetTasksAsync(Guid[]? id, Guid?[]? directionId, int? offset,
        int? limit, Guid currentUserId)
    {
        //If directionId is [null] return tasks created by the current user with direction id = null
        if (directionId is not null && directionId.Length == 1 && directionId.Contains(null))
        {
            return await _taskRepository.GetTasksAsync(id, directionId, offset, limit, currentUserId);
        }

        var authorizedDirectionIds = await GetUserDirectionIds(currentUserId);

        //If directionId is null return all tasks visible to the current user.
        //That includes tasks from all the directions where user is a member
        //(user is a member of his own directions)

        directionId = directionId == null
            ? authorizedDirectionIds.Append(null).ToArray()
            : directionId.Intersect(authorizedDirectionIds).ToArray();

        var tasks = await _taskRepository.GetTasksAsync(id, directionId, offset, limit, currentUserId);

        var tasksList = tasks.ToList();

        tasksList.ForEach(t1 =>
            t1.NumberOfRepetitions = t1.GroupId == null ? null : tasksList.Count(t2 => t2.GroupId == t1.GroupId));

        return tasksList;
    }

    private async Task<IEnumerable<Guid?>> GetUserDirectionIds(Guid currentUserId)
    {
        var userDirections = await _directionRepository.GetUserDirectionsAsync(currentUserId);
        return userDirections.Select(d => d?.Id);
    }
}