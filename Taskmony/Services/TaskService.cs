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
    private readonly INotificationService _notificationService;
    private readonly ITimeConverter _timeConverter;

    public TaskService(ITaskRepository taskRepository, IDirectionService directionService,
        INotificationService notificationService, ITimeConverter timeConverter)
    {
        _taskRepository = taskRepository;
        _directionService = directionService;
        _notificationService = notificationService;
        _timeConverter = timeConverter;
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
            tasks = (await _taskRepository.GetAsync(id, directionId, offsetValue, limitValue, currentUserId))
                .ToList();
        }
        else
        {
            var userDirectionIds = await _directionService.GetUserDirectionIdsAsync(currentUserId);
            var authorizedDirectionIds = userDirectionIds.Cast<Guid?>().Append(null);

            //If directionId is null return all tasks visible to the current user.
            //That includes tasks from all the directions where user is a member
            //(user is a member of his own directions)

            directionId = directionId == null
                ? authorizedDirectionIds.ToArray()
                : directionId.Intersect(authorizedDirectionIds).ToArray();

            tasks = (await _taskRepository.GetAsync(id, directionId, offsetValue, limitValue, currentUserId))
                .ToList();
        }

        return tasks;
    }

    public async Task<IEnumerable<Task>> GetTasksByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _taskRepository.GetByIdsAsync(ids);
    }

    public async Task<Task?> AddTaskAsync(Task task)
    {
        if (task.DirectionId is not null &&
            !await _directionService.AnyMemberWithIdAsync(task.DirectionId.Value, task.CreatedById))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        await ValidateAssignee(task);

        SetAssigneeAndAssigner(task);

        await _taskRepository.AddAsync(task);

        if (!await _taskRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityAddedAsync(task, task.CreatedAt, task.CreatedById);

        return task;
    }

    private void SetAssigneeAndAssigner(Task task)
    {
        if (task.DirectionId is not null && task.AssigneeId is null)
        {
            task.AssigneeId = task.CreatedById;
            task.AssignedById = task.CreatedById;
        }

        if (task.AssigneeId is not null)
        {
            task.AssignedById = task.CreatedById;
        }
    }

    public async Task<IEnumerable<Guid>> AddRecurringTaskAsync(Task task, RepeatMode repeatMode, int repeatEvery,
        WeekDay? weekDays, DateTime repeatUntil)
    {
        if (task is { RepeatMode: RepeatMode.Week, WeekDays: null })
        {
            throw new DomainException(ValidationErrors.WeekDaysAreRequired);
        }

        await ValidateAssignee(task);

        if (task.StartAt > repeatUntil)
        {
            throw new DomainException(ValidationErrors.RepeatUntilIsBeforeStartAt);
        }

        if (task.DirectionId is not null && task.AssigneeId is null)
        {
            task.AssigneeId = task.CreatedById;
            task.AssignedById = task.CreatedById;
        }

        var tasks = CreateRecurringTaskInstances(task, repeatMode, repeatEvery, weekDays, repeatUntil);

        await _taskRepository.AddRangeAsync(tasks);

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityAddedAsync(task, task.CreatedAt, task.CreatedById);

        return tasks.Select(t => t.Id);
    }

    private List<Task> CreateRecurringTaskInstances(Task task, RepeatMode repeatMode, int repeatEvery,
        WeekDay? weekDays, DateTime repeatUntil)
    {
        task.GroupId ??= Guid.NewGuid();
        task.CreatedAt ??= DateTime.UtcNow;

        if (weekDays is null || repeatMode != RepeatMode.Week)
        {
            return GenerateTasks(task, repeatUntil, repeatMode, repeatEvery);
        }

        return GenerateWeeklyTasks(task, repeatUntil, repeatMode, weekDays.Value, repeatEvery);
    }

    private List<Task> GenerateWeeklyTasks(Task task, DateTime repeatUntil, RepeatMode repeatMode,
        WeekDay weekDays, int repeatEvery)
    {
        var tasks = new List<Task>();
        var initialStartAt = task.StartAt!.Value;

        foreach (var day in Enum.GetValues(typeof(WeekDay)))
        {
            if (!weekDays.HasFlag((WeekDay)day))
            {
                continue;
            }

            var startAt = GetNextWeekday(initialStartAt, WeekDayToDayOfWeek((WeekDay)day));

            // Assuming that even if there is no fitting date on the current week, we start from the current week
            if (startAt > initialStartAt && startAt.DayOfWeek <= initialStartAt.DayOfWeek)
            {
                startAt = startAt.AddDays(7 * (repeatEvery - 1));
            }

            task = new Task
            {
                Description = Description.From(task.Description!.Value),
                Details = task.Details,
                CreatedById = task.CreatedById,
                CreatedAt = task.CreatedAt,
                StartAt = startAt,
                AssigneeId = task.AssigneeId,
                AssignedById = task.AssignedById,
                DirectionId = task.DirectionId,
                RepeatMode = task.RepeatMode,
                RepeatEvery = task.RepeatEvery,
                WeekDays = task.WeekDays,
                RepeatUntil = task.RepeatUntil,
                GroupId = task.GroupId
            };

            tasks.AddRange(GenerateTasks(task, repeatUntil, repeatMode, repeatEvery));
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

    private List<Task> GenerateTasks(Task task, DateTime repeatUntil, RepeatMode repeatMode, int repeatEvery)
    {
        var tasks = new List<Task>();
        var nextStartAt = task.StartAt!.Value;

        while (nextStartAt <= repeatUntil)
        {
            tasks.Add(new Task
            {
                Description = Description.From(task.Description!.Value),
                Details = task.Details,
                CreatedById = task.CreatedById,
                CreatedAt = task.CreatedAt,
                StartAt = nextStartAt,
                AssigneeId = task.AssigneeId,
                AssignedById = task.AssignedById,
                DirectionId = task.DirectionId,
                RepeatMode = task.RepeatMode,
                RepeatEvery = task.RepeatEvery,
                WeekDays = task.WeekDays,
                RepeatUntil = task.RepeatUntil,
                GroupId = task.GroupId
            });

            nextStartAt = GetNextDateTime(nextStartAt, repeatMode, repeatEvery);
        }

        return tasks;
    }

    private static DateTime GetNextDateTime(DateTime prev, RepeatMode repeatMode, int repeatEvery)
    {
        return repeatMode switch
        {
            RepeatMode.Day => prev.AddDays(repeatEvery),
            RepeatMode.Week => prev.AddDays(7 * repeatEvery),
            RepeatMode.Month => prev.AddMonths(repeatEvery),
            RepeatMode.Year => prev.AddYears(repeatEvery),
            _ => throw new DomainException(ValidationErrors.InvalidRepeatMode)
        };
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDescriptionAsync(Guid taskId, Guid groupId,
        string description, Guid currentUserId)
    {
        var newDescription = Description.From(description);
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        ValidateTasksToUpdate(tasks);

        var task = tasks.FirstOrDefault(t => t.Id == taskId && t.GroupId == groupId);

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        var oldValue = task.Description!.Value;

        tasks.ForEach(t => t.Description = Description.From(newDescription.Value));

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.Description), oldValue,
            description, currentUserId);

        return tasks.Select(t => t.Id);
    }

    public async Task<Guid?> SetTaskDescriptionAsync(Guid taskId, string description, Guid currentUserId)
    {
        var newDescription = Description.From(description);
        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        ValidateTaskToUpdate(task);

        var oldValue = task.Description!.Value;
        task.Description = newDescription;

        if (!await _taskRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.Description), oldValue,
            description, currentUserId);

        return task.Id;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDetailsAsync(Guid taskId, Guid groupId, string? details,
        Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        ValidateTasksToUpdate(tasks);

        var task = tasks.FirstOrDefault(t => t.Id == taskId && t.GroupId == groupId);

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        var oldValue = task.Details;

        tasks.ForEach(t => t.Details = details);

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.Details), oldValue, details,
            currentUserId);

        return tasks.Select(t => t.Id);
    }

    public async Task<Guid?> SetTaskDetailsAsync(Guid taskId, string? details, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        ValidateTaskToUpdate(task);

        var oldValue = task.Details;
        task.Details = details;

        if (!await _taskRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.Details), oldValue, details,
            currentUserId);

        return task.Id;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDirectionAsync(Guid taskId, Guid groupId, Guid? directionId,
        Guid currentUserId)
    {
        if (directionId is not null &&
            !await _directionService.AnyMemberWithIdAsync(directionId.Value, currentUserId))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        ValidateTasksToUpdate(tasks);

        var task = tasks.FirstOrDefault(t => t.Id == taskId && t.GroupId == groupId);

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        var oldDirectionId = task.DirectionId;

        tasks.ForEach(t => t.DirectionId = directionId);

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityMovedAsync(task, oldDirectionId, currentUserId, null);

        return tasks.Select(t => t.Id);
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

        var oldDirectionId = task.DirectionId;
        task.DirectionId = directionId;

        if (!await _taskRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityMovedAsync(task, oldDirectionId, currentUserId, null);

        return task.Id;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskAssigneeAsync(Guid taskId, Guid groupId, Guid? assigneeId,
        Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        ValidateTasksToUpdate(tasks);

        var task = tasks.FirstOrDefault(t => t.Id == taskId && t.GroupId == groupId);

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        var oldAssigneeId = task.AssigneeId;

        tasks.ForEach(t => t.AssigneeId = assigneeId);

        await ValidateAssignee(task);

        if (assigneeId is not null)
        {
            tasks.ForEach(t => t.AssignedById = currentUserId);
        }

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyTaskAssigneeUpdatedAsync(task, oldAssigneeId, currentUserId, null);

        return tasks.Select(t => t.Id);
    }

    public async Task<Guid?> SetTaskAssigneeAsync(Guid taskId, Guid? assigneeId, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        ValidateTaskToUpdate(task);

        var oldAssigneeId = task.AssigneeId;
        task.AssigneeId = assigneeId;

        await ValidateAssignee(task);

        if (assigneeId is not null)
        {
            task.AssignedById = currentUserId;
        }

        if (!await _taskRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyTaskAssigneeUpdatedAsync(task, oldAssigneeId, currentUserId, null);

        return task.Id;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskStartAtAsync(Guid taskId, Guid groupId, DateTime startAtUtc,
        Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        ValidateTasksToUpdate(tasks);

        var task = tasks.FirstOrDefault(t => t.Id == taskId && t.GroupId == groupId);

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        ValidateRepeatMode(task.RepeatMode, task.WeekDays, startAtUtc, task.RepeatUntil, task.RepeatEvery);

        var oldValue = _timeConverter.DateTimeToString(task.StartAt!.Value);
        var newValue = _timeConverter.DateTimeToString(startAtUtc);

        _taskRepository.DeleteRange(tasks);

        var taskToAdd = new Task
        {
            GroupId = task.GroupId,
            CreatedById = task.CreatedById,
            Description = Description.From(task.Description!.Value),
            Details = task.Details,
            AssigneeId = task.AssigneeId,
            DirectionId = task.DirectionId,
            StartAt = startAtUtc,
            RepeatMode = task.RepeatMode,
            RepeatEvery = task.RepeatEvery,
            WeekDays = task.WeekDays,
            RepeatUntil = task.RepeatUntil,
            AssignedById = task.AssignedById,
            CreatedAt = task.CreatedAt
        };

        var newTasks = CreateRecurringTaskInstances(taskToAdd, task.RepeatMode!.Value, task.RepeatEvery!.Value,
            task.WeekDays, task.RepeatUntil!.Value);

        await _taskRepository.AddRangeAsync(newTasks);

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.StartAt), oldValue, newValue,
            currentUserId);

        return (await _taskRepository.GetTasksByGroupIdAsync(groupId)).Select(t => t.Id);
    }

    public async Task<Guid?> SetTaskStartAtAsync(Guid taskId, DateTime startAtUtc, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        ValidateTaskToUpdate(task);

        var oldValue = _timeConverter.DateTimeToString(task.StartAt!.Value);
        var newValue = _timeConverter.DateTimeToString(startAtUtc);

        task.StartAt = startAtUtc;

        if (!await _taskRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.StartAt), oldValue, newValue,
            currentUserId);

        return task.Id;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDeletedAtAsync(Guid taskId, Guid groupId,
        DateTime? deletedAtUtc, Guid currentUserId, bool all)
    {
        var deletedAt = deletedAtUtc is not null ? DeletedAt.From(deletedAtUtc.Value) : null;

        var tasks = all
            ? await GetTasksOrThrowAsync(groupId, currentUserId)
            : await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        if (deletedAtUtc is not null && tasks.All(t => t.DeletedAt is not null))
        {
            throw new DomainException(TaskErrors.AlreadyDeleted);
        }

        var task = tasks.FirstOrDefault(t => t.Id == taskId && t.GroupId == groupId);

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        var oldValue = task.DeletedAt?.Value;
        var newValue = deletedAt?.Value;

        tasks.ForEach(t => t.DeletedAt = deletedAt);

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityDeletedAtUpdatedAsync(task, oldValue, newValue,
            currentUserId);

        return tasks.Select(t => t.Id);
    }

    public async Task<Guid?> SetTaskDeletedAtAsync(Guid taskId, DateTime? deletedAtUtc, Guid currentUserId)
    {
        var deletedAt = deletedAtUtc is not null ? DeletedAt.From(deletedAtUtc.Value) : null;

        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        if (deletedAtUtc is not null && task.DeletedAt is not null)
        {
            throw new DomainException(TaskErrors.AlreadyDeleted);
        }

        var oldValue = task.DeletedAt?.Value;
        var newValue = deletedAt?.Value;

        task.DeletedAt = deletedAt;

        if (!await _taskRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityDeletedAtUpdatedAsync(task, oldValue, newValue,
            currentUserId);

        return task.Id;
    }

    public async Task<Guid?> SetTaskCompletedAtAsync(Guid taskId, DateTime? completedAtUtc, Guid currentUserId)
    {
        var completedAt = completedAtUtc is not null ? CompletedAt.From(completedAtUtc.Value) : null;

        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

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

        var oldValue = task.CompletedAt is null ? null : _timeConverter.DateTimeToString(task.CompletedAt.Value);
        var newValue = completedAt is null ? null : _timeConverter.DateTimeToString(completedAt.Value);

        task.CompletedAt = completedAt;

        if (!await _taskRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.CompletedAt), oldValue, newValue,
            currentUserId);

        return task.Id;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskRepeatModeAsync(Guid taskId, Guid groupId,
        RepeatMode? repeatMode, WeekDay? weekDays, DateTime? startAt, DateTime? repeatUntil, int? repeatEvery,
        Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        ValidateTasksToUpdate(tasks);

        var task = tasks.FirstOrDefault(t => t.Id == taskId && t.GroupId == groupId);

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        if (repeatMode is null)
        {
            _taskRepository.DeleteRange(tasks);
        }
        else
        {
            var newStartAt = startAt ?? tasks.Select(t => t.StartAt).Min();

            ValidateRepeatMode(repeatMode, weekDays, newStartAt, repeatUntil, repeatEvery);

            _taskRepository.DeleteRange(tasks);

            var taskToAdd = new Task
            {
                GroupId = task.GroupId,
                CreatedById = task.CreatedById,
                Description = Description.From(task.Description!.Value),
                Details = task.Details,
                AssigneeId = task.AssigneeId,
                DirectionId = task.DirectionId,
                StartAt = newStartAt,
                RepeatMode = task.RepeatMode,
                RepeatEvery = task.RepeatEvery,
                WeekDays = task.WeekDays,
                RepeatUntil = task.RepeatUntil,
                AssignedById = task.AssignedById,
                CreatedAt = task.CreatedAt
            };

            var newTasks = CreateRecurringTaskInstances(taskToAdd, repeatMode.Value, repeatEvery!.Value,
                task.WeekDays, repeatUntil!.Value);

            await _taskRepository.AddRangeAsync(newTasks);
        }

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.RepeatMode),
            task.RepeatMode.ToString(), repeatMode?.ToString(), currentUserId);

        return (await _taskRepository.GetTasksByGroupIdAsync(groupId)).Select(t => t.Id);
    }

    public async Task<IEnumerable<Guid>> SetTaskRepeatModeAsync(Guid taskId, RepeatMode? repeatMode,
        WeekDay? weekDays, DateTime? startAt, DateTime? repeatUntil, int? repeatEvery, Guid currentUserId)
    {
        ValidateRepeatMode(repeatMode, weekDays, repeatUntil, repeatEvery);

        var task = await GetTaskOrThrowAsync(taskId, currentUserId);
        var newStartAt = startAt ?? task.StartAt!.Value;

        if (newStartAt > repeatUntil)
        {
            throw new DomainException(ValidationErrors.RepeatUntilIsBeforeStartAt);
        }

        ValidateTaskToUpdate(task);

        if (task.GroupId is not null)
        {
            return await SetRecurringTaskRepeatModeAsync(taskId, task.GroupId.Value, repeatMode, weekDays, startAt,
                repeatUntil, repeatEvery, currentUserId);
        }

        _taskRepository.Delete(task);

        task.RepeatMode = repeatMode;
        task.RepeatEvery = repeatEvery;
        task.WeekDays = weekDays;
        task.RepeatUntil = repeatUntil;
        task.StartAt = newStartAt;

        var tasks = CreateRecurringTaskInstances(task, repeatMode!.Value, repeatEvery!.Value, weekDays,
            repeatUntil!.Value);

        await _taskRepository.AddRangeAsync(tasks);

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.RepeatMode), null,
            repeatMode.ToString(), currentUserId);

        return tasks.Select(t => t.Id);
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskRepeatUntilAsync(Guid taskId, Guid groupId,
        DateTime repeatUntil, Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        ValidateTasksToUpdate(tasks);

        var task = tasks.FirstOrDefault(t => t.Id == taskId && t.GroupId == groupId);

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        if (task.StartAt > repeatUntil)
        {
            throw new DomainException(ValidationErrors.RepeatUntilIsBeforeStartAt);
        }

        if (repeatUntil <= task.RepeatUntil)
        {
            tasks.ForEach(t => t.RepeatUntil = repeatUntil);

            var tasksToDelete = tasks.Where(t => t.StartAt > repeatUntil).ToList();

            _taskRepository.DeleteRange(tasksToDelete);
        }
        else
        {
            tasks.ForEach(t => t.RepeatUntil = repeatUntil);

            var newTask = new Task
            {
                GroupId = task.GroupId,
                CreatedAt = task.CreatedAt,
                CreatedById = task.CreatedById,
                Description = Description.From(task.Description!.Value),
                Details = task.Details,
                AssigneeId = task.AssigneeId,
                AssignedById = task.AssignedById,
                DirectionId = task.DirectionId,
                // Add one day to the last task start date to avoid creating a new task with the same start date
                StartAt = tasks.Select(t => t.StartAt).Max()!.Value.AddDays(1),
                RepeatMode = task.RepeatMode,
                RepeatEvery = task.RepeatEvery,
                WeekDays = task.WeekDays,
                RepeatUntil = repeatUntil
            };

            var tasksToAdd = CreateRecurringTaskInstances(newTask, task.RepeatMode!.Value, task.RepeatEvery!.Value,
                task.WeekDays, repeatUntil);

            await _taskRepository.AddRangeAsync(tasksToAdd);
        }

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        var oldValue = _timeConverter.DateTimeToString(task.RepeatUntil!.Value);
        var newValue = _timeConverter.DateTimeToString(repeatUntil);

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.RepeatUntil), oldValue, newValue,
            currentUserId);

        return (await _taskRepository.GetTasksByGroupIdAsync(groupId)).Select(t => t.Id);
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
        var task = await _taskRepository.GetByIdAsync(id);

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

    private void ValidateRepeatMode(RepeatMode? repeatMode, WeekDay? weekDays, DateTime? startAt,
        DateTime? repeatUntil, int? repeatEvery)
    {
        ValidateRepeatMode(repeatMode, weekDays, repeatUntil, repeatEvery);

        if (repeatUntil < startAt)
        {
            throw new DomainException(ValidationErrors.RepeatUntilIsBeforeStartAt);
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