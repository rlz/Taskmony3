using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Tasks;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly INotificationService _notificationService;
    private readonly ITimeConverter _timeConverter;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IRecurringTaskGenerator _recurringTaskGenerator;

    public TaskService(ITaskRepository taskRepository, IDirectionRepository directionRepository,
        IAssignmentRepository assignmentRepository, INotificationService notificationService,
        ITimeConverter timeConverter, ICommentRepository commentRepository,
        IRecurringTaskGenerator recurringTaskGenerator)
    {
        _taskRepository = taskRepository;
        _directionRepository = directionRepository;
        _notificationService = notificationService;
        _timeConverter = timeConverter;
        _assignmentRepository = assignmentRepository;
        _commentRepository = commentRepository;
        _recurringTaskGenerator = recurringTaskGenerator;
    }

    public async Task<IEnumerable<Task>> GetTasksAsync(Guid[]? id, Guid?[]? directionId, bool? completed, bool? deleted,
        DateTime? lastCompletedAt, DateTime? lastDeletedAt, int? offset, int? limit, Guid currentUserId)
    {
        int? limitValue = limit == null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset == null ? null : Offset.From(offset.Value).Value;
        List<Task> tasks;

        // If directionId is [null] return tasks created by the current user with direction id = null
        if (directionId != null && directionId.Length == 1 && directionId.Contains(null))
        {
            tasks = (await _taskRepository.GetAsync(
                id: id,
                directionId: directionId,
                completed: completed,
                deleted: deleted,
                lastCompletedAt: lastCompletedAt,
                lastDeletedAt: lastDeletedAt,
                offset: offsetValue,
                limit: limitValue,
                userId: currentUserId)).ToList();
        }
        else
        {
            var userDirectionIds = await _directionRepository.GetUserDirectionIdsAsync(currentUserId);
            var authorizedDirectionIds = userDirectionIds.Cast<Guid?>().Append(null);

            // If directionId is null return all tasks visible to the current user.
            // That includes tasks from all the directions where user is a member
            // (user is a member of his own directions)

            directionId = directionId == null
                ? authorizedDirectionIds.ToArray()
                : directionId.Intersect(authorizedDirectionIds).ToArray();

            tasks = (await _taskRepository.GetAsync(
                id: id,
                directionId: directionId,
                completed: completed,
                deleted: deleted,
                lastCompletedAt: lastCompletedAt,
                lastDeletedAt: lastDeletedAt,
                offset: offsetValue,
                limit: limitValue,
                userId: currentUserId)).ToList();
        }

        return tasks;
    }

    public async Task<IEnumerable<Task>> GetTasksByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _taskRepository.GetByIdsAsync(ids);
    }

    public async Task<Task?> AddTaskAsync(string description, string? details, Guid? assigneeId, Guid? directionId,
        DateTime startAtUtc, Guid currentUserId)
    {
        var task = new Task(
            description: Description.From(description),
            details: Details.From(details),
            createdById: currentUserId,
            startAt: startAtUtc,
            assignment: assigneeId != null
                ? new Assignment(assigneeId.Value, currentUserId)
                : null,
            directionId: directionId);

        await ValidateTaskDirection(task, currentUserId);

        await ValidateAssignee(task, task.Assignment);

        await _taskRepository.AddAsync(task);

        if (!await _taskRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityAddedAsync(task, task.CreatedAt, task.CreatedById);

        return task;
    }

    public async Task<IEnumerable<Guid>> AddRecurringTaskAsync(string description, string? details, Guid? assigneeId,
        Guid? directionId, DateTime startAtUtc, RepeatMode repeatMode, int repeatEvery, WeekDay? weekDays,
        DateTime repeatUntilUtc, Guid currentUserId)
    {
        var pattern = new RecurrencePattern(repeatMode, weekDays, repeatEvery, repeatUntilUtc);

        var task = new Task(
            description: Description.From(description),
            details: Details.From(details),
            createdById: currentUserId,
            startAt: startAtUtc,
            directionId: directionId,
            assignment: assigneeId != null
                ? new Assignment(assigneeId.Value, currentUserId)
                : null,
            groupId: null,
            recurrencePattern: pattern);

        await ValidateTaskDirection(task, currentUserId);

        await ValidateAssignee(task, task.Assignment);

        var tasks = _recurringTaskGenerator.CreateRecurringTaskInstances(task, pattern);

        await _taskRepository.AddRangeAsync(tasks);

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityAddedAsync(
            tasks.First(t => t.StartAt == tasks.Min(x => x.StartAt)), task.CreatedAt, task.CreatedById);

        return tasks.Select(t => t.Id);
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDescriptionAsync(Guid taskId, Guid groupId,
        string description, Guid currentUserId)
    {
        var newDescription = Description.From(description);

        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);
        var task = GetTaskFromGroupOrThrow(tasks, groupId, taskId);

        var oldValue = task.Description!.Value;

        tasks.ForEach(t => t.UpdateDescription(Description.From(newDescription.Value)));

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

        var oldValue = task.Description!.Value;
        task.UpdateDescription(newDescription);

        return await UpdateTaskAsync(task, nameof(Task.Description), oldValue, description, currentUserId)
            ? taskId
            : null;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDetailsAsync(Guid taskId, Guid groupId, string? details,
        Guid currentUserId)
    {
        var newDetails = Details.From(details);
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);
        var task = GetTaskFromGroupOrThrow(tasks, groupId, taskId);

        var oldValue = task.Details?.Value;

        tasks.ForEach(t => t.UpdateDetails(Details.From(newDetails.Value)));

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
        var newDetails = Details.From(details);
        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        var oldValue = task.Details?.Value;
        task.UpdateDetails(newDetails);

        return await UpdateTaskAsync(task, nameof(Task.Details), oldValue, details, currentUserId) ? task.Id : null;
    }

    /// <summary>
    /// Saves changes, removes recurring instance task from the group if present and creates notification
    /// </summary>
    /// <param name="task">task to update</param>
    /// <param name="fieldName">updated field</param>
    /// <param name="oldValue">old value of the updated field</param>
    /// <param name="newValue">new value of the updated field</param>
    /// <param name="currentUserId">id of the current user</param>
    /// <returns>whether any changes were made</returns>
    private async Task<bool> UpdateTaskAsync(Task task, string fieldName, string? oldValue, string? newValue,
        Guid currentUserId)
    {
        if (!await UpdateTaskAsync(task))
        {
            return false;
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, fieldName, oldValue, newValue,
            currentUserId);

        return true;
    }

    /// <summary>
    /// Saves changes and removes recurring instance task from the group if present
    /// </summary>
    /// <param name="task">task to update</param>
    /// <returns>whether any changes were made</returns>
    private async Task<bool> UpdateTaskAsync(Task task)
    {
        // Remove recurring task instance from the group when updating one instance only
        // to avoid further changes to this instance with the group 
        if (task.GroupId != null)
        {
            task.RemoveFromGroup();
        }

        return await _taskRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDirectionAsync(Guid taskId, Guid groupId, Guid? directionId,
        Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);
        var task = GetTaskFromGroupOrThrow(tasks, groupId, taskId);

        task.UpdateDirectionId(directionId);
        
        await ValidateTaskDirection(task, currentUserId);

        var oldDirectionId = task.DirectionId;

        tasks.ForEach(t => t.UpdateDirectionId(directionId));

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityMovedAsync(task, oldDirectionId, currentUserId, null);

        return tasks.Select(t => t.Id);
    }

    public async Task<Guid?> SetTaskDirectionAsync(Guid taskId, Guid? directionId, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(taskId, currentUserId);
        
        var oldDirectionId = task.DirectionId;
        task.UpdateDirectionId(directionId);
        
        await ValidateTaskDirection(task, currentUserId);

        if (!await UpdateTaskAsync(task))
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
        var task = GetTaskFromGroupOrThrow(tasks, groupId, taskId);

        var oldAssigneeId = task.Assignment?.AssigneeId;

        if (assigneeId == oldAssigneeId)
        {
            return Array.Empty<Guid>();
        }

        var newAssignment = assigneeId != null ? new Assignment(assigneeId.Value, currentUserId) : null;

        await ValidateAssignee(task, newAssignment);
        
        task.UpdateAssignment(newAssignment);

        await _assignmentRepository.UpdateAssignmentAsync(tasks, newAssignment);

        await _notificationService.NotifyTaskAssigneeUpdatedAsync(task, oldAssigneeId, currentUserId, null);

        return tasks.Select(t => t.Id);
    }

    public async Task<Guid?> SetTaskAssigneeAsync(Guid taskId, Guid? assigneeId, Guid currentUserId)
    {
        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        var oldAssigneeId = task.Assignment?.AssigneeId;

        if (assigneeId == oldAssigneeId)
        {
            return null;
        }

        var newAssignment = assigneeId != null ? new Assignment(assigneeId.Value, currentUserId) : null;
        
        await ValidateAssignee(task, newAssignment);
        
        task.UpdateAssignment(newAssignment);

        if (!await _assignmentRepository.UpdateAssignmentAsync(task, newAssignment))
        {
            return null;
        }

        await UpdateTaskAsync(task);

        await _notificationService.NotifyTaskAssigneeUpdatedAsync(task, oldAssigneeId, currentUserId, null);

        return task.Id;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskStartAtAsync(Guid taskId, Guid groupId, DateTime startAtUtc,
        Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);
        var task = GetTaskFromGroupOrThrow(tasks, groupId, taskId);

        var pattern = task.RecurrencePattern!;

        if (pattern.RepeatUntil < startAtUtc)
        {
            throw new DomainException(ValidationErrors.RepeatUntilIsBeforeStartAt);
        }

        var oldValue = _timeConverter.DateTimeToString(task.StartAt!.Value);
        var newValue = _timeConverter.DateTimeToString(startAtUtc);

        _taskRepository.DeleteRange(tasks);

        var taskToAdd = new Task(
            description: Description.From(task.Description!.Value),
            details: Details.From(task.Details?.Value),
            createdById: task.CreatedById,
            startAt: startAtUtc,
            assignment: task.Assignment == null
                ? null
                : new Assignment(task.Assignment.AssigneeId, task.Assignment.AssignedById),
            recurrencePattern: new RecurrencePattern(pattern.RepeatMode, pattern.WeekDays, pattern.RepeatEvery,
                pattern.RepeatUntil),
            groupId: task.GroupId,
            directionId: task.DirectionId);

        var newTasks = _recurringTaskGenerator.CreateRecurringTaskInstances(taskToAdd, pattern);

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

        var oldValue = _timeConverter.DateTimeToString(task.StartAt!.Value);
        var newValue = _timeConverter.DateTimeToString(startAtUtc);

        task.UpdateStartAt(startAtUtc);

        return await UpdateTaskAsync(task, nameof(Task.StartAt), oldValue, newValue, currentUserId) ? task.Id : null;
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskDeletedAtAsync(Guid taskId, Guid groupId,
        DateTime? deletedAtUtc, Guid currentUserId, bool all)
    {
        var deletedAt = deletedAtUtc != null ? DeletedAt.From(deletedAtUtc.Value) : null;

        var tasks = all
            ? await GetTasksOrThrowAsync(groupId, currentUserId)
            : await GetActiveTasksOrThrowAsync(groupId, currentUserId);

        var task = GetTaskFromGroupOrThrow(tasks, groupId, taskId);

        var oldValue = task.DeletedAt?.Value;
        var newValue = deletedAt?.Value;

        tasks.ForEach(t => t.UpdateDeletedAt(deletedAt != null ? DeletedAt.From(deletedAt.Value) : null));

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        if (deletedAt != null)
        {
            await _commentRepository.SoftDeleteTaskCommentsAsync(tasks.Select(t => t.Id));
        }
        else if (oldValue != null)
        {
            await _commentRepository.UndeleteIdeaCommentsAsync(tasks.Select(t => t.Id), oldValue.Value);
        }

        await _notificationService.NotifyDirectionEntityDeletedAtUpdatedAsync(task, oldValue, newValue,
            currentUserId);

        return tasks.Select(t => t.Id);
    }

    public async Task<Guid?> SetTaskDeletedAtAsync(Guid taskId, DateTime? deletedAtUtc, Guid currentUserId)
    {
        var deletedAt = deletedAtUtc != null ? DeletedAt.From(deletedAtUtc.Value) : null;

        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        var oldValue = task.DeletedAt?.Value;
        var newValue = deletedAt?.Value;

        task.UpdateDeletedAt(deletedAt);

        // TODO: delete task with comments in one transaction
        if (!await UpdateTaskAsync(task))
        {
            return null;
        }

        if (deletedAt != null)
        {
            await _commentRepository.SoftDeleteTaskCommentsAsync(new[] {task.Id});
        }
        else if (oldValue != null)
        {
            await _commentRepository.UndeleteTaskCommentsAsync(new[] {task.Id}, oldValue.Value);
        }

        await _notificationService.NotifyDirectionEntityDeletedAtUpdatedAsync(task, oldValue, newValue,
            currentUserId);

        return task.Id;
    }

    public async Task<Guid?> SetTaskCompletedAtAsync(Guid taskId, DateTime? completedAtUtc, Guid currentUserId)
    {
        var completedAt = completedAtUtc != null ? CompletedAt.From(completedAtUtc.Value) : null;

        var task = await GetTaskOrThrowAsync(taskId, currentUserId);

        var oldValue = task.CompletedAt is null ? null : _timeConverter.DateTimeToString(task.CompletedAt.Value);
        var newValue = completedAt is null ? null : _timeConverter.DateTimeToString(completedAt.Value);

        task.UpdateCompletedAt(completedAt);

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
        var task = GetTaskFromGroupOrThrow(tasks, groupId, taskId);

        if (repeatMode is null)
        {
            _taskRepository.DeleteRange(tasks);
        }
        else
        {
            var pattern = new RecurrencePattern(repeatMode, weekDays, repeatEvery, repeatUntil);
            var newStartAt = startAt ?? tasks.Select(t => t.StartAt).Min();

            _taskRepository.DeleteRange(tasks);

            var taskToAdd = new Task(
                description: Description.From(task.Description!.Value),
                details: Details.From(task.Details?.Value),
                createdById: task.CreatedById,
                groupId: task.GroupId,
                directionId: task.DirectionId,
                startAt: newStartAt!.Value,
                assignment: task.Assignment != null
                    ? new Assignment(task.Assignment.AssigneeId, task.Assignment.AssignedById)
                    : null,
                recurrencePattern: pattern,
                createdAt: task.CreatedAt);

            var newTasks = _recurringTaskGenerator.CreateRecurringTaskInstances(taskToAdd, pattern);

            await _taskRepository.AddRangeAsync(newTasks);
        }

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.RecurrencePattern.RepeatMode),
            task.RecurrencePattern!.RepeatMode.ToString(), repeatMode?.ToString(), currentUserId);

        return (await _taskRepository.GetTasksByGroupIdAsync(groupId)).Select(t => t.Id);
    }

    public async Task<IEnumerable<Guid>> SetTaskRepeatModeAsync(Guid taskId, RepeatMode? repeatMode,
        WeekDay? weekDays, DateTime? startAt, DateTime? repeatUntil, int? repeatEvery, Guid currentUserId)
    {
        var pattern = new RecurrencePattern(repeatMode, weekDays, repeatEvery, repeatUntil);

        var task = await GetTaskOrThrowAsync(taskId, currentUserId);
        var newStartAt = startAt ?? task.StartAt!.Value;

        if (task.GroupId != null)
        {
            return await SetRecurringTaskRepeatModeAsync(taskId, task.GroupId.Value, repeatMode, weekDays, startAt,
                repeatUntil, repeatEvery, currentUserId);
        }
        
        task.UpdateStartAt(newStartAt);
        task.UpdateRecurrencePattern(pattern);
        task.UpdateGroupId(Guid.NewGuid());

        var tasks = _recurringTaskGenerator.CreateRecurringTaskInstances(task, pattern);

        // Remove duplicate task
        tasks.RemoveAll(t => t.StartAt == task.StartAt);

        await _taskRepository.AddRangeAsync(tasks);

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.RecurrencePattern.RepeatMode),
            null,
            repeatMode.ToString(), currentUserId);

        return tasks.Select(t => t.Id);
    }

    public async Task<IEnumerable<Guid>> SetRecurringTaskRepeatUntilAsync(Guid taskId, Guid groupId,
        DateTime repeatUntil, Guid currentUserId)
    {
        var tasks = await GetActiveTasksOrThrowAsync(groupId, currentUserId);
        var task = GetTaskFromGroupOrThrow(tasks, groupId, taskId);

        if (repeatUntil <= task.RecurrencePattern!.RepeatUntil)
        {
            tasks.ForEach(t => t.RecurrencePattern!.UpdateRepeatUntil(repeatUntil));

            var tasksToDelete = tasks.Where(t => t.StartAt > repeatUntil).ToList();

            _taskRepository.DeleteRange(tasksToDelete);
        }
        else
        {
            tasks.ForEach(t => t.RecurrencePattern!.UpdateRepeatUntil(repeatUntil));
            var lastStartAt = tasks.Select(t => t.StartAt).Max()!.Value;

            var newTask = new Task(
                description: Description.From(task.Description!.Value),
                details: Details.From(task.Details?.Value),
                createdById: task.CreatedById,
                startAt: lastStartAt,
                groupId: task.GroupId,
                assignment: task.Assignment != null
                    ? new Assignment(task.Assignment.AssigneeId, task.Assignment.AssignedById)
                    : null,
                directionId: task.DirectionId,
                recurrencePattern: new RecurrencePattern(
                    task.RecurrencePattern!.RepeatMode,
                    task.RecurrencePattern.WeekDays,
                    task.RecurrencePattern.RepeatEvery,
                    repeatUntil),
                createdAt: task.CreatedAt);

            var tasksToAdd = _recurringTaskGenerator.CreateRecurringTaskInstances(newTask, newTask.RecurrencePattern!);
            
            // Remove duplicate task
            tasksToAdd.RemoveAll(t => t.StartAt == lastStartAt);

            await _taskRepository.AddRangeAsync(tasksToAdd);
        }

        if (!await _taskRepository.SaveChangesAsync())
        {
            return Array.Empty<Guid>();
        }

        var oldValue = _timeConverter.DateTimeToString(task.RecurrencePattern.RepeatUntil);
        var newValue = _timeConverter.DateTimeToString(repeatUntil);

        await _notificationService.NotifyDirectionEntityUpdatedAsync(task, nameof(Task.RecurrencePattern.RepeatUntil),
            oldValue, newValue, currentUserId);

        return (await _taskRepository.GetTasksByGroupIdAsync(groupId)).Select(t => t.Id);
    }

    public async Task<bool> RemoveAssigneeFromDirectionTasksAsync(Guid assigneeId, Guid directionId)
    {
        var tasks = await _taskRepository.GetByDirectionIdAndAssigneeIdAsync(directionId, assigneeId);

        tasks.ToList().ForEach(t => t.UpdateAssignment(null));

        return await _taskRepository.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task SoftDeleteDirectionTasksAsync(Guid directionId)
    {
        await _taskRepository.SoftDeleteDirectionTasksAndCommentsAsync(directionId);
    }

    public async System.Threading.Tasks.Task UndeleteDirectionTasksAsync(Guid directionId, DateTime deletedAt)
    {
        await _taskRepository.UndeleteDirectionTasksAndComments(directionId, deletedAt);
    }

    private Task GetTaskFromGroupOrThrow(IEnumerable<Task> tasks, Guid groupId, Guid taskId)
    {
        var task = tasks.FirstOrDefault(t => t.Id == taskId && t.GroupId == groupId);

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        return task;
    }

    private async System.Threading.Tasks.Task ValidateTaskDirection(Task task, Guid currentUserId)
    {
        if (task.DirectionId != null)
        {
            var direction = await _directionRepository.GetByIdAsync(task.DirectionId.Value);

            if (direction == null ||
                !await _directionRepository.AnyMemberWithIdAsync(task.DirectionId.Value, currentUserId))
            {
                throw new DomainException(DirectionErrors.NotFound);
            }

            direction.ValidateDirectionToUpdate();
        }
    }

    private async Task<List<Task>> GetTasksOrThrowAsync(Guid groupId, Guid currentUserId)
    {
        var tasks = (await _taskRepository.GetTasksByGroupIdAsync(groupId)).ToList();
        var task = tasks.FirstOrDefault();

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        if (!await UserHasAccess(task, currentUserId))
        {
            throw new DomainException(GeneralErrors.Forbidden);
        }

        return tasks;
    }

    private async Task<List<Task>> GetActiveTasksOrThrowAsync(Guid groupId, Guid currentUserId)
    {
        var tasks = (await _taskRepository.GetActiveTasksAsync(groupId)).ToList();
        var task = tasks.FirstOrDefault();

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        if (!await UserHasAccess(task, currentUserId))
        {
            throw new DomainException(GeneralErrors.Forbidden);
        }

        return tasks;
    }

    public async Task<Task> GetTaskOrThrowAsync(Guid id, Guid currentUserId)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
        {
            throw new DomainException(TaskErrors.NotFound);
        }

        if (!await UserHasAccess(task, currentUserId))
        {
            throw new DomainException(GeneralErrors.Forbidden);
        }

        return task;
    }

    private async Task<bool> UserHasAccess(Task task, Guid currentUserId)
    {
        // Task should either be created by the current user or 
        // belong to a direction where the current user is a member
        return task.CreatedById == currentUserId && task.DirectionId == null ||
               task.DirectionId != null &&
               await _directionRepository.AnyMemberWithIdAsync(task.DirectionId.Value, currentUserId);
    }

    private async System.Threading.Tasks.Task ValidateAssignee(Task task, Assignment? assignment)
    {
        if (assignment == null)
        {
            return;
        }

        if (task.DirectionId != null &&
            !await _directionRepository.AnyMemberWithIdAsync(task.DirectionId.Value, assignment.AssigneeId))
        {
            throw new DomainException(DirectionErrors.MemberNotFound);
        }
    }
}