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

    public Details? Details { get; private set; }

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

    public Task(Description description, Details details, Guid createdById, DateTime startAt, Assignment? assignment,
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

        if (assignment != null && directionId == null)
        {
            throw new DomainException(TaskErrors.AssignPrivateTask);
        }

        if (directionId != null && assignment == null)
        {
            Assignment = new Assignment(createdById, createdById);
        }
    }

    public Task(Guid id, Description description, Details details, Guid createdById, DateTime startAt,
        Assignment? assignment, Guid? directionId, DateTime? createdAt = null, CompletedAt? completedAt = null,
        DeletedAt? deletedAt = null) : this(description, details, createdById, startAt, assignment, directionId,
        createdAt, completedAt, deletedAt)
    {
        Id = id;
    }

    public Task(Description description, Details details, Guid createdById, DateTime startAt, Assignment? assignment,
        Guid? directionId, RecurrencePattern? recurrencePattern, Guid? groupId, DateTime? createdAt = null,
        CompletedAt? completedAt = null, DeletedAt? deletedAt = null) : this(description, details, createdById, startAt,
        assignment, directionId, createdAt, completedAt, deletedAt)
    {
        RecurrencePattern = recurrencePattern;
        GroupId = groupId;

        if (recurrencePattern != null)
        {
            ValidateRecurrenceInterval(recurrencePattern.RepeatMode, startAt, recurrencePattern.RepeatUntil);
        }
    }

    public Task(Guid id, Description description, Details details, Guid createdById, DateTime startAt,
        Assignment? assignment, Guid? directionId, RecurrencePattern? recurrencePattern, Guid? groupId,
        DateTime? createdAt = null, CompletedAt? completedAt = null, DeletedAt? deletedAt = null) : this(description,
        details, createdById, startAt, assignment, directionId, recurrencePattern, groupId, createdAt, completedAt,
        deletedAt)
    {
        Id = id;
    }

    public void ValidateTaskToUpdate()
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

    public void UpdateDetails(Details details)
    {
        ValidateTaskToUpdate();

        Details = details;
    }

    public void UpdateStartAt(DateTime startAt)
    {
        ValidateTaskToUpdate();

        if (RecurrencePattern != null)
        {
            ValidateRecurrenceInterval(RecurrencePattern.RepeatMode, startAt, RecurrencePattern.RepeatUntil);
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

        if (recurrencePattern != null)
        {
            ValidateRecurrenceInterval(recurrencePattern.RepeatMode, StartAt!.Value, recurrencePattern.RepeatUntil);
        }

        RecurrencePattern = recurrencePattern;
    }

    public void RemoveFromGroup()
    {
        GroupId = null;
        RecurrencePattern = null;
    }

    public void UpdateDirectionId(Guid? directionId)
    {
        ValidateTaskToUpdate();

        DirectionId = directionId;
    }

    private void ValidateRecurrenceInterval(RepeatMode repeatMode, DateTime startAt, DateTime repeatUntil)
    {
        if (startAt > repeatUntil)
        {
            throw new DomainException(ValidationErrors.RepeatUntilIsBeforeStartAt);
        }

        if ((repeatMode == RepeatMode.Day || repeatMode == RepeatMode.Week) && startAt.AddYears(3) < repeatUntil)
        {
            throw new DomainException(ValidationErrors.RepeatUntilIsTooFarForDailyAndWeekly);
        }

        if ((repeatMode == RepeatMode.Month || repeatMode == RepeatMode.Year) && startAt.AddYears(100) < repeatUntil)
        {
            throw new DomainException(ValidationErrors.RepeatUntilIsTooFarForMonthlyAndYearly);
        }
    }
}