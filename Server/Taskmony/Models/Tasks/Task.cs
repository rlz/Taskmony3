using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Comments;
using Taskmony.Models.Directions;
using Taskmony.Models.Notifications;
using Taskmony.Models.Subscriptions;
using Taskmony.Models.Users;
using Taskmony.Models.ValueObjects;

namespace Taskmony.Models.Tasks;

public class Task : DirectionEntity
{
    public override ActionItemType ActionItemType => ActionItemType.Task;

    public Description? Description { get; private set; }

    public string? Details { get; private set; }

    public User? CreatedBy { get; private set; }

    public Guid CreatedById { get; private set; }

    public DateTime? CreatedAt { get; private set; }

    public DateTime? StartAt { get; private set; }

    public CompletedAt? CompletedAt { get; private set; }

    public DeletedAt? DeletedAt { get; private set; }

    public Assignment? Assignment { get; private set; }

    public RecurrencePattern? RecurrencePattern { get; private set; }

    public Guid? GroupId { get; private set; }

    public ICollection<TaskComment>? Comments { get; private set; }

    public ICollection<Notification>? Notifications { get; private set; }

    public ICollection<TaskSubscription>? Subscriptions { get; private set; }

    // Required by EF Core
    private Task()
    {
    }

    public Task(Description description, string? details, Guid createdById, DateTime? startAt, Assignment? assignment,
        Guid? directionId, DateTime? createdAt = null, CompletedAt? completedAt = null, DeletedAt? deletedAt = null)
    {
        Description = description;
        Details = details;
        CreatedById = createdById;
        StartAt = startAt;
        Assignment = assignment;
        DirectionId = directionId;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        CompletedAt = completedAt;
        DeletedAt = deletedAt;

        if (directionId != null && assignment == null)
        {
            Assignment = new Assignment(createdById, createdById);
        }
    }

    public Task(Description description, string? details, Guid createdById, DateTime? startAt, Assignment? assignment,
        Guid? directionId, RecurrencePattern? recurrencePattern, Guid? groupId, DateTime? createdAt = null,
        CompletedAt? completedAt = null, DeletedAt? deletedAt = null) : this(description, details, createdById, startAt,
        assignment, directionId, createdAt, completedAt, deletedAt)
    {
        RecurrencePattern = recurrencePattern;
        GroupId = groupId;
    }

    private void ValidateTaskToUpdate()
    {
        if (CompletedAt != null || DeletedAt != null)
        {
            throw new DomainException(TaskErrors.UpdateCompletedOrDeletedTask);
        }
    }

    public void UpdateDescription(Description description)
    {
        ValidateTaskToUpdate();

        Description = description;
    }

    public void UpdateDetails(string? details)
    {
        ValidateTaskToUpdate();

        Details = details;
    }

    public void UpdateStartAt(DateTime startAt)
    {
        ValidateTaskToUpdate();

        if (RecurrencePattern != null && startAt > RecurrencePattern.RepeatUntil)
        {
            throw new DomainException(ValidationErrors.EndBeforeStart);
        }

        StartAt = startAt;
    }

    public void UpdateCompletedAt(CompletedAt? completedAt)
    {
        if (completedAt != null && CompletedAt != null)
        {
            throw new DomainException(TaskErrors.AlreadyCompleted);
        }

        if (completedAt != null && DeletedAt != null)
        {
            throw new DomainException(TaskErrors.CompleteDeletedTask);
        }

        CompletedAt = completedAt;
    }

    public void UpdateAssignment(Assignment? assignment)
    {
        ValidateTaskToUpdate();

        if (assignment != null && DirectionId == null)
        {
            throw new DomainException(TaskErrors.AssignPrivateTask);
        }

        Assignment = assignment;
    }

    public void UpdateDeletedAt(DeletedAt? deletedAt)
    {
        if (deletedAt != null && DeletedAt != null)
        {
            throw new DomainException(TaskErrors.AlreadyDeleted);
        }

        DeletedAt = deletedAt;
    }

    public void UpdateGroupId(Guid? groupId)
    {
        if (groupId != null && RecurrencePattern == null)
        {
            throw new DomainException(TaskErrors.GroupWithoutRecurrence);
        }

        GroupId = groupId;
    }

    public void UpdateRecurrencePattern(RecurrencePattern? recurrencePattern)
    {
        ValidateTaskToUpdate();

        RecurrencePattern = recurrencePattern;
    }

    public void RemoveTaskFromGroup()
    {
        GroupId = null;
        RecurrencePattern = null;
    }

    public void UpdateDirectionId(Guid? directionId)
    {
        ValidateTaskToUpdate();

        DirectionId = directionId;
    }
}