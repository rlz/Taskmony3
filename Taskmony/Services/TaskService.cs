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

    public async Task<IEnumerable<Task>> GetTasksByIdsAsync(Guid[] ids)
    {
        return await _taskRepository.GetTasksByIdsAsync(ids);
    }

    public async Task<Task?> AddTaskAsync(Task task)
    {
        ValidateTaskDescription(task.Description);

        if (task.DirectionId is not null &&
            !await _directionService.AnyMemberWithIdAsync(task.DirectionId.Value, task.CreatedById))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        if (task.AssigneeId is not null)
        {
            if (task.DirectionId is null)
            {
                throw new DomainException(TaskErrors.DirectionIsMissing);
            }

            if (!await _directionService.AnyMemberWithIdAsync(task.DirectionId.Value, task.AssigneeId.Value))
            {
                throw new DomainException(TaskErrors.InvalidAssignee);
            }
        }

        await _taskRepository.AddTaskAsync(task);

        if (await _taskRepository.SaveChangesAsync())
        {
            return task;
        }

        return null;
    }

    public async Task<IEnumerable<Guid>> AddRecurringTaskAsync(Task task, RepeatMode repeatMode, int? repeatEvery,
        int numberOfRepetitions)
    {
        ValidateTaskDescription(task.Description);

        if (task is {RepeatMode: RepeatMode.Custom, RepeatEvery: null})
        {
            throw new DomainException(TaskErrors.RepeatEveryIsMissing);
        }

        var tasks = GenerateTasks(task, repeatMode, repeatEvery, numberOfRepetitions);

        await _taskRepository.AddTasksAsync(tasks);

        return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
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
            _ => throw new DomainException(ValidationErrors.InvalidRepeatMode)
        };
    }

    public async Task<IEnumerable<Guid>> SetTaskDescriptionAsync(Guid? taskId, Guid? groupId,
        string description, Guid currentUserId)
    {
        if (taskId is null && groupId is null)
        {
            throw new DomainException(ValidationErrors.TaskIdOrGroupIdIsRequired);
        }

        ValidateTaskDescription(description);

        if (groupId is not null)
        {
            var tasks = await GetActiveTasksOrThrowAsync(groupId.Value, currentUserId);

            ValidateTasksToUpdate(tasks);

            tasks.ForEach(t => t.Description = description);

            return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
        }

        var task = await GetTaskOrThrowAsync(taskId!.Value, currentUserId);

        ValidateTaskToUpdate(task);

        task.Description = description;

        return await _taskRepository.SaveChangesAsync() ? new[] {taskId.Value} : Array.Empty<Guid>();
    }

    public async Task<IEnumerable<Guid>> SetTaskDetailsAsync(Guid? taskId, Guid? groupId, string? details,
        Guid currentUserId)
    {
        if (taskId is null && groupId is null)
        {
            throw new DomainException(ValidationErrors.TaskIdOrGroupIdIsRequired);
        }

        if (groupId is not null)
        {
            var tasks = await GetActiveTasksOrThrowAsync(groupId.Value, currentUserId);

            ValidateTasksToUpdate(tasks);

            tasks.ForEach(t => t.Details = details);

            return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
        }

        var task = await GetTaskOrThrowAsync(taskId!.Value, currentUserId);

        ValidateTaskToUpdate(task);

        task.Details = details;

        return await _taskRepository.SaveChangesAsync() ? new[] {taskId.Value} : Array.Empty<Guid>();
    }

    public async Task<IEnumerable<Guid>> SetTaskDirectionAsync(Guid? taskId, Guid? groupId, Guid? directionId,
        Guid currentUserId)
    {
        if (taskId is null && groupId is null)
        {
            throw new DomainException(ValidationErrors.TaskIdOrGroupIdIsRequired);
        }

        if (directionId is not null &&
            !await _directionService.AnyMemberWithIdAsync(directionId.Value, currentUserId))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        if (groupId is not null)
        {
            var tasks = await GetActiveTasksOrThrowAsync(groupId.Value, currentUserId);

            ValidateTasksToUpdate(tasks);

            tasks.ForEach(t => t.DirectionId = directionId);

            return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
        }

        var task = await GetTaskOrThrowAsync(taskId!.Value, currentUserId);

        ValidateTaskToUpdate(task);

        task.DirectionId = directionId;

        return await _taskRepository.SaveChangesAsync() ? new[] {taskId.Value} : Array.Empty<Guid>();
    }

    public async Task<IEnumerable<Guid>> SetTaskAssigneeAsync(Guid? taskId, Guid? groupId, Guid? assigneeId,
        Guid currentUserId)
    {
        if (taskId is null && groupId is null)
        {
            throw new DomainException(ValidationErrors.TaskIdOrGroupIdIsRequired);
        }

        if (groupId is not null)
        {
            var tasks = await GetActiveTasksOrThrowAsync(groupId.Value, currentUserId);

            ValidateTasksToUpdate(tasks);
            await ValidateAssignee(tasks.First(), assigneeId);

            tasks.ForEach(t => t.AssigneeId = assigneeId);

            return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
        }

        var task = await GetTaskOrThrowAsync(taskId!.Value, currentUserId);

        ValidateTaskToUpdate(task);
        await ValidateAssignee(task, assigneeId);

        task.AssigneeId = assigneeId;

        return await _taskRepository.SaveChangesAsync() ? new[] {taskId.Value} : Array.Empty<Guid>();
    }

    public async Task<IEnumerable<Guid>> SetTaskStartAtAsync(Guid? taskId, Guid? groupId, DateTime startAtUtc,
        Guid currentUserId)
    {
        if (taskId is null && groupId is null)
        {
            throw new DomainException(ValidationErrors.TaskIdOrGroupIdIsRequired);
        }

        if (groupId is not null)
        {
            var tasks = await GetActiveTasksOrThrowAsync(groupId.Value, currentUserId);

            ValidateTasksToUpdate(tasks);

            tasks.ForEach(t => t.StartAt = startAtUtc);

            return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
        }

        var task = await GetTaskOrThrowAsync(taskId!.Value, currentUserId);

        ValidateTaskToUpdate(task);

        task.StartAt = startAtUtc;

        return await _taskRepository.SaveChangesAsync() ? new[] {taskId.Value} : Array.Empty<Guid>();
    }

    public async Task<IEnumerable<Guid>> SetTaskDeletedAtAsync(Guid? taskId, Guid? groupId, DateTime? deletedAtUtc,
        Guid currentUserId)
    {
        if (taskId is null && groupId is null)
        {
            throw new DomainException(ValidationErrors.TaskIdOrGroupIdIsRequired);
        }

        ValidateDeletedAt(deletedAtUtc);

        if (groupId is not null)
        {
            var tasks = deletedAtUtc is not null
                ? await GetActiveTasksOrThrowAsync(groupId.Value, currentUserId)
                : await GetNotCompletedTasksOrThrowAsync(groupId.Value, currentUserId);

            tasks.ForEach(t => t.DeletedAt = deletedAtUtc);

            return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
        }

        var task = await GetTaskOrThrowAsync(taskId!.Value, currentUserId);

        if (deletedAtUtc is not null && task.DeletedAt is not null)
        {
            throw new DomainException(TaskErrors.AlreadyDeleted);
        }

        ValidateDeletedAt(deletedAtUtc);

        task.DeletedAt = deletedAtUtc;

        return await _taskRepository.SaveChangesAsync() ? new[] {taskId.Value} : Array.Empty<Guid>();
    }

    public async Task<Guid?> SetTaskCompletedAtAsync(Guid id, DateTime? completedAtUtc, Guid currentUserId)
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

        return await _taskRepository.SaveChangesAsync() ? id : null;
    }

    private async Task<List<Task>> GetNotCompletedTasksOrThrowAsync(Guid groupId, Guid currentUserId)
    {
        var tasks = (await _taskRepository.GetNotCompletedTasksAsync(groupId)).ToList();
        var task = tasks.FirstOrDefault();

        if (await IsNotNullAndUserHasAccess(task, currentUserId))
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        return tasks;
    }

    private async Task<List<Task>> GetActiveTasksOrThrowAsync(Guid groupId, Guid currentUserId)
    {
        var tasks = (await _taskRepository.GetActiveTasksAsync(groupId)).ToList();
        var task = tasks.FirstOrDefault();

        if (await IsNotNullAndUserHasAccess(task, currentUserId))
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        return tasks;
    }

    public async Task<Task> GetTaskOrThrowAsync(Guid id, Guid currentUserId)
    {
        var task = await _taskRepository.GetTaskByIdAsync(id);

        if (await IsNotNullAndUserHasAccess(task, currentUserId))
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        return task!;
    }

    private async Task<bool> IsNotNullAndUserHasAccess(Task? task, Guid currentUserId)
    {
        //Task should either be created by the current user or 
        //belong to a direction where the current user is a member
        return task is null ||
               task.CreatedById != currentUserId && task.DirectionId == null ||
               task.DirectionId != null &&
               !await _directionService.AnyMemberWithIdAsync(task.DirectionId.Value, currentUserId);
    }

    private void ValidateTaskDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException(ValidationErrors.InvalidDescription);
        }
    }

    private void ValidateTaskToUpdate(Task task)
    {
        ValidateTasksToUpdate(new List<Task> {task});
    }

    private void ValidateTasksToUpdate(List<Task> tasks)
    {
        if (tasks.All(t => t.CompletedAt is not null) || tasks.All(t => t.DeletedAt is not null))
        {
            throw new DomainException(TaskErrors.UpdateCompletedOrDeletedTask);
        }
    }

    private void ValidateDeletedAt(DateTime? deletedAtUtc)
    {
        if (deletedAtUtc is not null && deletedAtUtc > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidDeletedAt);
        }
    }

    private async System.Threading.Tasks.Task ValidateAssignee(Task task, Guid? assigneeId)
    {
        if (task.DirectionId is null)
        {
            throw new DomainException(TaskErrors.AssignPrivateTask);
        }

        if (assigneeId is not null &&
            !await _directionService.AnyMemberWithIdAsync(task.DirectionId.Value, assigneeId.Value))
        {
            throw new DomainException(DirectionErrors.MemberNotFound);
        }
    }
}