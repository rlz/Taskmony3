using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Enums;
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

    public async Task<Task> AddTask(Task task)
    {
        if (task.DirectionId is not null &&
            !await _directionService.AnyMemberWithId(task.DirectionId.Value, task.CreatedById))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        if (task.AssigneeId is not null)
        {
            if (task.DirectionId is null)
            {
                throw new DomainException(TaskErrors.DirectionIsMissing);
            }

            if (!await _directionService.AnyMemberWithId(task.DirectionId.Value, task.AssigneeId.Value))
            {
                throw new DomainException(TaskErrors.InvalidAssignee);
            }
        }

        await _taskRepository.AddTask(task);

        await _taskRepository.SaveChangesAsync();

        return task;
    }

    public async Task<Guid[]> AddRepeatingTask(Task task, RepeatMode repeatMode, int? repeatEvery, int numberOfRepetitions)
    {
        if (task.RepeatMode == RepeatMode.Custom && task.RepeatEvery is null)
        {
            throw new DomainException(TaskErrors.RepeatEveryIsMissing);
        }

        var tasks = GenerateTasks(task, repeatMode, repeatEvery, numberOfRepetitions);

        await _taskRepository.AddTasks(tasks);

        await _taskRepository.SaveChangesAsync();

        return tasks.Select(t => t.Id).ToArray();
    }

    private List<Task> GenerateTasks(Task task, RepeatMode repeatMode, int? repeatEvery, int numberOfRepetitions)
    {
        var groupId = Guid.NewGuid();
        var tasks = new List<Task>();

        task.GroupId = groupId;
        task.StartAt ??= DateTime.UtcNow;

        tasks.Add(task);

        for (int i = 1; i < numberOfRepetitions; i++)
        {
            task = new Task
            {
                Description = task.Description,
                Details = task.Details,
                DirectionId = task.DirectionId,
                AssigneeId = task.AssigneeId,
                CreatedById = task.CreatedById,
                GroupId = groupId,
                RepeatMode = repeatMode,
                RepeatEvery = repeatEvery,
                StartAt = GetNextDateTime(task.StartAt!.Value, repeatMode, repeatEvery)
            };

            tasks.Add(task);
        }

        return tasks;
    }

    private DateTime GetNextDateTime(DateTime prev, RepeatMode repeatMode, int? repeatEvery)
    {
        return repeatMode switch
        {
            RepeatMode.Day => prev.AddDays(1),
            RepeatMode.Week => prev.AddDays(7),
            RepeatMode.Month => prev.AddMonths(1),
            RepeatMode.Year => prev.AddYears(1),
            RepeatMode.Custom => prev.AddDays(repeatEvery!.Value),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task<bool> SetTaskDescription(Guid id, string description, Guid currentUserId)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException(ValidationErrors.InvalidDescription);
        }

        var task = await GetTaskOrThrowAsync(id, currentUserId);

        if (task.CompletedAt is not null || task.DeletedAt is not null)
        {
            throw new DomainException(TaskErrors.UpdateCompletedOrDeletedTask);
        }

        task.Description = description;

        return await _taskRepository.SaveChangesAsync();
    }

    public async Task<bool> SetTaskDetails(Guid id, string? details, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(id, currentUserId);

        if (task.CompletedAt is not null || task.DeletedAt is not null)
        {
            throw new DomainException(TaskErrors.UpdateCompletedOrDeletedTask);
        }

        task.Details = details;

        return await _taskRepository.SaveChangesAsync();
    }

    public async Task<bool> SetTaskDirection(Guid id, Guid? directionId, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(id, currentUserId);

        if (task.CompletedAt is not null || task.DeletedAt is not null)
        {
            throw new DomainException(TaskErrors.UpdateCompletedOrDeletedTask);
        }

        if (directionId is not null &&
            !await _directionService.AnyMemberWithId(directionId.Value, currentUserId))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        task.DirectionId = directionId;

        return await _taskRepository.SaveChangesAsync();
    }

    public async Task<bool> SetTaskAssignee(Guid id, Guid? assigneeId, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(id, currentUserId);

        if (task.DirectionId is null)
        {
            throw new DomainException(TaskErrors.DirectionIsMissing);
        }

        if (task.CompletedAt is not null || task.DeletedAt is not null)
        {
            throw new DomainException(TaskErrors.UpdateCompletedOrDeletedTask);
        }

        if (assigneeId is not null &&
            !await _directionService.AnyMemberWithId(task.DirectionId.Value, assigneeId.Value))
        {
            throw new DomainException(DirectionErrors.MemberNotFound);
        }

        task.AssigneeId = assigneeId;

        return await _taskRepository.SaveChangesAsync();
    }

    public async Task<bool> SetTaskStartAt(Guid id, DateTime startAtUtc, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(id, currentUserId);

        if (task.CompletedAt is not null || task.DeletedAt is not null)
        {
            throw new DomainException(TaskErrors.UpdateCompletedOrDeletedTask);
        }

        task.StartAt = startAtUtc;

        return await _taskRepository.SaveChangesAsync();
    }

    public async Task<bool> SetTaskDeletedAt(Guid id, DateTime? deletedAtUtc, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(id, currentUserId);

        if (deletedAtUtc is not null && task.DeletedAt is not null)
        {
            throw new DomainException(TaskErrors.AlreadyDeleted);
        }

        if (deletedAtUtc is not null && task.StartAt > deletedAtUtc)
        {
            throw new DomainException(TaskErrors.DeleteFutureTask);
        }

        if (deletedAtUtc is not null && deletedAtUtc > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidDeletedAt);
        }

        task.DeletedAt = deletedAtUtc;

        return await _taskRepository.SaveChangesAsync();
    }

    public async Task<bool> SetTaskCompletedAt(Guid id, DateTime? completedAtUtc, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(id, currentUserId);

        if (completedAtUtc is not null && task.DeletedAt is not null)
        {
            throw new DomainException(TaskErrors.CompleteDeletedTask);
        }

        if (completedAtUtc is not null && task.StartAt > completedAtUtc)
        {
            throw new DomainException(TaskErrors.CompleteFutureTask);
        }

        if (completedAtUtc is not null && task.CompletedAt is not null)
        {
            throw new DomainException(TaskErrors.AlreadyCompleted);
        }

        if (completedAtUtc is not null && completedAtUtc > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidCompletedAt);
        }

        task.CompletedAt = completedAtUtc;

        return await _taskRepository.SaveChangesAsync();
    }

    public async Task<Guid[]> SetRecurringTaskDeletedAt(Guid groupId, DateTime? deletedAtUtc, Guid currentUserId)
    {
        var tasks = await GetTasksOrThrowAsync(groupId, currentUserId);

        if (deletedAtUtc is not null && tasks.All(t => t.DeletedAt is not null))
        {
            throw new DomainException(TaskErrors.AlreadyDeleted);
        }

        tasks.ForEach(t => t.DeletedAt = deletedAtUtc);

        await _taskRepository.SaveChangesAsync();

        return tasks.Select(t => t.Id).ToArray();
    }

    private async Task<List<Task>> GetTasksOrThrowAsync(Guid groupId, Guid currentUserId)
    {
        var tasks = (await _taskRepository.GetNotCompletedTasksByGroupIdAsync(groupId)).ToList();
        var task = tasks.FirstOrDefault();

        //Task should either be created by the current user or 
        //belong to a direction where the current user is a member
        if (task is null ||
            task.CreatedById != currentUserId && task.DirectionId == null ||
            task.DirectionId != null && !await _directionService.AnyMemberWithId(task.DirectionId.Value, currentUserId))
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        return tasks;
    }

    public async Task<Task> GetTaskOrThrowAsync(Guid id, Guid currentUserId)
    {
        var task = await _taskRepository.GetTaskByIdAsync(id);

        //Task should either be created by the current user or 
        //belong to a direction where the current user is a member
        if (task is null ||
            task.CreatedById != currentUserId && task.DirectionId == null ||
            task.DirectionId != null && !await _directionService.AnyMemberWithId(task.DirectionId.Value, currentUserId))
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        return task;
    }
}