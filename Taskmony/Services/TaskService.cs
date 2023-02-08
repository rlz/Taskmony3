using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Enums;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;
using Taskmony.ValueObjects;
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
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;
        List<Task> tasks;

        //If directionId is [null] return tasks created by the current user with direction id = null
        if (directionId is not null && directionId.Length == 1 && directionId.Contains(null))
        {
            tasks = (await _taskRepository.GetTasksAsync(id, directionId, offsetValue, limitValue, currentUserId))
                .ToList();
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

            tasks = (await _taskRepository.GetTasksAsync(id, directionId, offsetValue, limitValue, currentUserId))
                .ToList();
        }

        return tasks;
    }

    public async Task<IEnumerable<Task>> GetTasksByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _taskRepository.GetTasksByIdsAsync(ids);
    }

    public async Task<Task?> AddTaskAsync(Task task)
    {
        if (task.DirectionId is not null &&
            !await _directionService.AnyMemberWithIdAsync(task.DirectionId.Value, task.CreatedById))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        await ValidateAssignee(task);

        await _taskRepository.AddTaskAsync(task);

        return await _taskRepository.SaveChangesAsync() ? task : null;
    }

    public async Task<IEnumerable<Guid>> AddRecurringTaskAsync(Task task, RepeatMode repeatMode, int repeatEvery,
        WeekDay? weekDays, DateTime repeatUntil)
    {
        if (task is { RepeatMode: RepeatMode.Week, WeekDays: null })
        {
            throw new DomainException(ValidationErrors.WeekDaysAreRequired);
        }

        var repeatUntilValue = RepeatUntil.From(repeatUntil);
        var tasks = GenerateTasks(task, repeatMode, repeatEvery, weekDays, repeatUntilValue);

        await _taskRepository.AddTasksAsync(tasks);

        return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
    }

    private List<Task> GenerateTasks(Task task, RepeatMode repeatMode, int repeatEvery, WeekDay? weekDays,
        RepeatUntil repeatUntil)
    {
        var tasks = new List<Task>();

        task.GroupId ??= Guid.NewGuid();
        task.CreatedAt ??= DateTime.UtcNow;

        if (weekDays is null || repeatMode != RepeatMode.Week)
        {
            tasks.Add(task);
            tasks.AddRange(GenerateNextTasks(task, repeatUntil, repeatMode, repeatEvery));
            return tasks;
        }

        var initialStartAt = task.StartAt!.Value;

        foreach (var day in Enum.GetValues(typeof(WeekDay)))
        {
            if (weekDays.Value.HasFlag((WeekDay)day))
            {
                var startAt = GetNextWeekday(initialStartAt, WeekDayToDayOfWeek((WeekDay)day));

                if (startAt > initialStartAt && startAt.DayOfWeek <= initialStartAt.DayOfWeek)
                {
                    startAt = startAt.AddDays(7 * (repeatEvery - 1));
                }

                task = new Task
                {
                    Description = Description.From(task.Description!.Value),
                    Details = task.Details,
                    DirectionId = task.DirectionId,
                    StartAt = startAt,
                    RepeatMode = repeatMode,
                    RepeatEvery = repeatEvery,
                    WeekDays = weekDays,
                    RepeatUntil = RepeatUntil.From(repeatUntil.Value),
                    CreatedById = task.CreatedById,
                    CreatedAt = task.CreatedAt,
                    GroupId = task.GroupId
                };

                tasks.Add(task);
                tasks.AddRange(GenerateNextTasks(task, repeatUntil, repeatMode, repeatEvery));
            }
        }

        return tasks;
    }

    private DayOfWeek WeekDayToDayOfWeek(WeekDay weekDay)
    {
        return weekDay switch
        {
            WeekDay.Monday => DayOfWeek.Monday,
            WeekDay.Tuesday => DayOfWeek.Tuesday,
            WeekDay.Wednesday => DayOfWeek.Wednesday,
            WeekDay.Thursday => DayOfWeek.Thursday,
            WeekDay.Friday => DayOfWeek.Friday,
            WeekDay.Saturday => DayOfWeek.Saturday,
            WeekDay.Sunday => DayOfWeek.Sunday,
            _ => throw new ArgumentOutOfRangeException(nameof(weekDay), weekDay, null)
        };
    }

    private static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
    {
        return start.AddDays(((int)day - (int)start.DayOfWeek + 7) % 7);
    }

    private IEnumerable<Task> GenerateNextTasks(Task task, RepeatUntil repeatUntil, RepeatMode repeatMode,
        int repeatEvery)
    {
        var tasks = new List<Task>();
        var nextStartAt = GetNextDateTime(task.StartAt!.Value, task.RepeatMode!.Value, task.RepeatEvery!.Value);

        while (nextStartAt <= repeatUntil.Value)
        {
            tasks.Add(new Task
            {
                Description = Description.From(task.Description!.Value),
                Details = task.Details,
                DirectionId = task.DirectionId,
                AssigneeId = task.AssigneeId,
                CreatedById = task.CreatedById,
                GroupId = task.GroupId,
                RepeatMode = repeatMode,
                RepeatEvery = task.RepeatEvery,
                WeekDays = task.WeekDays,
                RepeatUntil = RepeatUntil.From(repeatUntil.Value),
                StartAt = nextStartAt,
                CreatedAt = task.CreatedAt
            });

            nextStartAt = GetNextDateTime(nextStartAt, repeatMode, repeatEvery);
        }

        return tasks;
    }

    private static DateTime GetNextDateTime(DateTime prev, RepeatMode repeatMode, int repeatEvery)
    {
        return repeatMode switch
        {
            RepeatMode.Day => prev.AddDays(1 * repeatEvery),
            RepeatMode.Week => prev.AddDays(7 * repeatEvery),
            RepeatMode.Month => prev.AddMonths(1 * repeatEvery),
            RepeatMode.Year => prev.AddYears(1 * repeatEvery),
            _ => throw new DomainException(ValidationErrors.InvalidRepeatMode)
        };
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDescriptionAsync(Guid groupId, string description,
        Guid currentUserId)
    {
        var newDescription = Description.From(description);

        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        ValidateTasksToUpdate(tasks);

        tasks.ForEach(t => t.Description = Description.From(newDescription.Value));

        return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
    }

    public async Task<Guid?> SetTaskDescriptionAsync(Guid taskId, string description, Guid currentUserId)
    {
        var newDescription = Description.From(description);

        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        ValidateTaskToUpdate(task);

        task.Description = newDescription;

        return await _taskRepository.SaveChangesAsync() ? taskId : null;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDetailsAsync(Guid groupId, string? details, Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        ValidateTasksToUpdate(tasks);

        tasks.ForEach(t => t.Details = details);

        return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
    }

    public async Task<Guid?> SetTaskDetailsAsync(Guid taskId, string? details, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        ValidateTaskToUpdate(task);

        task.Details = details;

        return await _taskRepository.SaveChangesAsync() ? taskId : null;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDirectionAsync(Guid groupId, Guid? directionId,
        Guid currentUserId)
    {
        if (directionId is not null &&
            !await _directionService.AnyMemberWithIdAsync(directionId.Value, currentUserId))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        ValidateTasksToUpdate(tasks);

        tasks.ForEach(t => t.DirectionId = directionId);

        return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
    }

    public async Task<Guid?> SetTaskDirectionAsync(Guid taskId, Guid? directionId, Guid currentUserId)
    {
        if (directionId is not null &&
            !await _directionService.AnyMemberWithIdAsync(directionId.Value, currentUserId))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        ValidateTaskToUpdate(task);

        task.DirectionId = directionId;

        return await _taskRepository.SaveChangesAsync() ? taskId : null;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskAssigneeAsync(Guid groupId, Guid? assigneeId,
        Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        ValidateTasksToUpdate(tasks);

        tasks.ForEach(t => t.AssigneeId = assigneeId);

        await ValidateAssignee(tasks.First());

        return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
    }

    public async Task<Guid?> SetTaskAssigneeAsync(Guid taskId, Guid? assigneeId, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        ValidateTaskToUpdate(task);

        task.AssigneeId = assigneeId;

        await ValidateAssignee(task);

        return await _taskRepository.SaveChangesAsync() ? taskId : null;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskStartAtAsync(Guid groupId, DateTime startAtUtc,
        Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        if (tasks.Count == 0)
        {
            return Array.Empty<Guid>();
        }

        ValidateTasksToUpdate(tasks);

        var firstTaskStartAt = tasks.Select(t => t.StartAt!.Value).Min();

        if (startAtUtc > firstTaskStartAt)
        {
            var tasksToDelete = tasks.Where(t => t.StartAt < startAtUtc).ToList();

            _taskRepository.DeleteTasks(tasksToDelete);

            return await _taskRepository.SaveChangesAsync() ? tasksToDelete.Select(t => t.Id) : Array.Empty<Guid>();
        }
        
        var task = new Task
        {
            GroupId = tasks.First().GroupId,
            CreatedById = tasks.First().CreatedById,
            Description = Description.From(tasks.First().Description!.Value),
            Details = tasks.First().Details,
            AssigneeId = tasks.First().AssigneeId,
            DirectionId = tasks.First().DirectionId,
            StartAt = firstTaskStartAt,
            RepeatMode = tasks.First().RepeatMode,
            RepeatEvery = tasks.First().RepeatEvery,
            WeekDays = tasks.First().WeekDays,
            RepeatUntil = RepeatUntil.From(tasks.First().RepeatUntil!.Value)
        };

        return await AddRecurringTaskAsync(task, tasks.First().RepeatMode!.Value, tasks.First().RepeatEvery!.Value,
            tasks.First().WeekDays, tasks.First().RepeatUntil!.Value);
    }

    public async Task<Guid?> SetTaskStartAtAsync(Guid taskId, DateTime startAtUtc, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        ValidateTaskToUpdate(task);

        task.StartAt = startAtUtc;

        return await _taskRepository.SaveChangesAsync() ? taskId : null;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDeletedAtAsync(Guid groupId, DateTime? deletedAtUtc,
        Guid currentUserId, bool all)
    {
        var deletedAt = deletedAtUtc is not null ? DeletedAt.From(deletedAtUtc.Value) : null;

        var tasks = all 
            ? await GetTasksOrThrowAsync(groupId, currentUserId) 
            : await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        if (deletedAtUtc is not null && tasks.All(t => t.DeletedAt is not null))
        {
            throw new DomainException(TaskErrors.AlreadyDeleted);
        }

        tasks.ForEach(t => t.DeletedAt = deletedAt);

        return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
    }

    public async Task<Guid?> SetTaskDeletedAtAsync(Guid taskId, DateTime? deletedAtUtc, Guid currentUserId)
    {
        var deletedAt = deletedAtUtc is not null ? DeletedAt.From(deletedAtUtc.Value) : null;

        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        if (deletedAtUtc is not null && task.DeletedAt is not null)
        {
            throw new DomainException(TaskErrors.AlreadyDeleted);
        }

        task.DeletedAt = deletedAt;

        return await _taskRepository.SaveChangesAsync() ? taskId : null;
    }

    public async Task<Guid?> SetTaskCompletedAtAsync(Guid id, DateTime? completedAtUtc, Guid currentUserId)
    {
        var completedAt = completedAtUtc is not null ? CompletedAt.From(completedAtUtc.Value) : null;

        var task = await GetTaskOrThrowAsync(id, currentUserId);

        if (completedAtUtc is not null && task.DeletedAt is not null)
        {
            throw new DomainException(TaskErrors.CompleteDeletedTask);
        }

        if (completedAtUtc is not null && task.StartAt!.Value > completedAtUtc)
        {
            throw new DomainException(TaskErrors.CompleteFutureTask);
        }

        if (completedAtUtc is not null && task.CompletedAt is not null)
        {
            throw new DomainException(TaskErrors.AlreadyCompleted);
        }

        task.CompletedAt = completedAt;

        return await _taskRepository.SaveChangesAsync() ? id : null;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskRepeatModeAsync(Guid groupId, RepeatMode? repeatMode,
        WeekDay? weekDays, DateTime? repeatUntil, int? repeatEvery, Guid currentUserId)
    {
        List<Task> tasks;

        if (repeatMode is null)
        {
            tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

            ValidateTasksToUpdate(tasks);

            _taskRepository.DeleteTasks(tasks);

            return await _taskRepository.SaveChangesAsync() ? tasks.Select(t => t.Id) : Array.Empty<Guid>();
        }

        ValidateRepeatMode(repeatMode, weekDays, repeatUntil, repeatEvery);

        var repeatUntilValue = RepeatUntil.From(repeatUntil!.Value).Value;
        tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        if (tasks.Count == 0)
        {
            return Array.Empty<Guid>();
        }

        ValidateTasksToUpdate(tasks);

        _taskRepository.DeleteTasks(tasks);

        var task = new Task
        {
            GroupId = tasks.First().GroupId,
            CreatedById = tasks.First().CreatedById,
            Description = Description.From(tasks.First().Description!.Value),
            Details = tasks.First().Details,
            AssigneeId = tasks.First().AssigneeId,
            DirectionId = tasks.First().DirectionId,
            StartAt = tasks.Select(t => t.StartAt).Min(),
            RepeatMode = repeatMode,
            RepeatEvery = repeatEvery,
            WeekDays = weekDays,
            RepeatUntil = RepeatUntil.From(repeatUntilValue)
        };

        return await AddRecurringTaskAsync(task, repeatMode.Value, repeatEvery!.Value, task.WeekDays, repeatUntil.Value);
    }

    public async Task<IEnumerable<Guid>> SetTaskRepeatModeAsync(Guid taskId, RepeatMode? repeatMode, WeekDay? weekDays, 
        DateTime? repeatUntil, int? repeatEvery, Guid currentUserId)
    {
        ValidateRepeatMode(repeatMode, weekDays, repeatUntil, repeatEvery);

        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        ValidateTaskToUpdate(task);

        task.RepeatMode = repeatMode;
        task.RepeatEvery = repeatEvery;
        task.WeekDays = weekDays;
        task.RepeatUntil = RepeatUntil.From(repeatUntil!.Value);

        return await AddRecurringTaskAsync(task, repeatMode!.Value, repeatEvery!.Value, weekDays, repeatUntil.Value);
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskRepeatUntilAsync(Guid groupId, DateTime repeatUntil, Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        if (tasks.Count == 0)
        {
            return Array.Empty<Guid>();
        }

        ValidateTasksToUpdate(tasks);

        if (repeatUntil < tasks.Select(t => t.RepeatUntil!.Value).Max())
        {
            var tasksToDelete = tasks.Where(t => t.StartAt > repeatUntil).ToList();

            _taskRepository.DeleteTasks(tasksToDelete);

            return await _taskRepository.SaveChangesAsync() ? tasksToDelete.Select(t => t.Id) : Array.Empty<Guid>();
        }

        tasks.ForEach(t => t.RepeatUntil = RepeatUntil.From(repeatUntil));

        var task = new Task
        {
            GroupId = tasks.First().GroupId,
            CreatedById = tasks.First().CreatedById,
            Description = Description.From(tasks.First().Description!.Value),
            Details = tasks.First().Details,
            AssigneeId = tasks.First().AssigneeId,
            DirectionId = tasks.First().DirectionId,
            StartAt = tasks.Select(t => t.StartAt).Min(),
            RepeatMode = tasks.First().RepeatMode,
            RepeatEvery = tasks.First().RepeatEvery,
            WeekDays = tasks.First().WeekDays,
            RepeatUntil = RepeatUntil.From(repeatUntil)
        };

        return await AddRecurringTaskAsync(task, tasks.First().RepeatMode!.Value, tasks.First().RepeatEvery!.Value,
            tasks.First().WeekDays, repeatUntil);
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

    private async Task<List<Task>> GetTasksOrThrowAsync(Guid groupId, Guid currentUserId)
    {
        var tasks = (await _taskRepository.GetTasksByGroupIdAsync(groupId)).ToList();
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

    private void ValidateRepeatMode(RepeatMode? repeatMode, WeekDay? weekDays, DateTime? repeatUntil, int? repeatEvery)
    {
        if (repeatMode is null)
        {
            throw new DomainException(ValidationErrors.RepeatModeIsRequired);
        }

        if (repeatEvery is null)
        {
            throw new DomainException(ValidationErrors.RepeatEveryIsRequired);
        }

        if (repeatMode == RepeatMode.Week && weekDays is null)
        {
            throw new DomainException(ValidationErrors.WeekDaysAreRequired);
        }

        if (repeatUntil is null)
        {
            throw new DomainException(ValidationErrors.RepeatUntilIsRequired);
        }
    }

    private void ValidateTaskToUpdate(Task task)
    {
        ValidateTasksToUpdate(new List<Task> { task });
    }

    private void ValidateTasksToUpdate(List<Task> tasks)
    {
        if (tasks.All(t => t.CompletedAt is not null) || tasks.All(t => t.DeletedAt is not null))
        {
            throw new DomainException(TaskErrors.UpdateCompletedOrDeletedTask);
        }
    }

    private async System.Threading.Tasks.Task ValidateAssignee(Task task)
    {
        if (task.AssigneeId is not null && task.DirectionId is null)
        {
            throw new DomainException(TaskErrors.AssignPrivateTask);
        }

        if (task.AssigneeId is not null && task.DirectionId is not null &&
            !await _directionService.AnyMemberWithIdAsync(task.DirectionId.Value, task.AssigneeId.Value))
        {
            throw new DomainException(DirectionErrors.MemberNotFound);
        }
    }
}