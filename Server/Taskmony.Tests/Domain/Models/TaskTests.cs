using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Notifications;
using Taskmony.Models.Tasks;
using Taskmony.Models.ValueObjects;

namespace Taskmony.Tests.Domain.Models;

using Task = Taskmony.Models.Tasks.Task;

public class TaskTests
{
    [Fact]
    public void CreateTask()
    {
        var startAt = DateTime.UtcNow;
        var userId = Guid.NewGuid();

        var task = new Task(
            description: Description.From("description"),
            details: Details.From("details"),
            createdById: userId,
            startAt: startAt,
            assignment: null,
            directionId: null);

        Assert.Equal(ActionItemType.Task, task.ActionItemType);
        Assert.NotNull(task.Description);
        Assert.NotNull(task.Details);
        Assert.NotNull(task.CreatedAt);
        Assert.NotNull(task.StartAt);
        Assert.Null(task.CompletedAt);
        Assert.Null(task.DeletedAt);
        Assert.Null(task.Assignment);
        Assert.Null(task.DirectionId);
        Assert.Null(task.GroupId);
        Assert.Equal(Description.From("description"), task.Description);
        Assert.Equal(Details.From("details"), task.Details);
        Assert.Equal(startAt, task.StartAt);
        Assert.Equal(userId, task.CreatedById);
    }

    [Fact]
    public void CreateTaskWithDirection()
    {
        var startAt = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();

        var task = new Task(
            description: Description.From("description"),
            details: Details.From("details"),
            createdById: userId,
            startAt: startAt,
            assignment: null,
            directionId: directionId);

        Assert.Equal(ActionItemType.Task, task.ActionItemType);
        Assert.NotNull(task.Description);
        Assert.NotNull(task.Details);
        Assert.NotNull(task.CreatedAt);
        Assert.NotNull(task.StartAt);
        Assert.Null(task.CompletedAt);
        Assert.Null(task.DeletedAt);
        Assert.NotNull(task.Assignment);
        Assert.NotNull(task.DirectionId);
        Assert.Null(task.GroupId);
        Assert.Equal(Description.From("description"), task.Description);
        Assert.Equal(Details.From("details"), task.Details);
        Assert.Equal(startAt, task.StartAt);
        Assert.Equal(userId, task.CreatedById);
        Assert.Equal(directionId, task.DirectionId);
        Assert.Equal(userId, task.Assignment!.AssigneeId);
        Assert.Equal(userId, task.Assignment!.AssignedById);
    }

    [Fact]
    public void CreateTask_WithAssignment()
    {
        var startAt = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var directionId = Guid.NewGuid();

        var task = new Task(
            description: Description.From("description"),
            details: Details.From("details"),
            createdById: userId,
            startAt: startAt,
            assignment: new Assignment(assigneeId, userId),
            directionId: directionId);

        Assert.Equal(ActionItemType.Task, task.ActionItemType);
        Assert.NotNull(task.Description);
        Assert.NotNull(task.Details);
        Assert.NotNull(task.CreatedAt);
        Assert.NotNull(task.StartAt);
        Assert.Null(task.CompletedAt);
        Assert.Null(task.DeletedAt);
        Assert.NotNull(task.Assignment);
        Assert.Null(task.GroupId);
        Assert.Equal(directionId, task.DirectionId);
        Assert.Equal(Description.From("description"), task.Description);
        Assert.Equal(Details.From("details"), task.Details);
        Assert.Equal(startAt, task.StartAt);
        Assert.Equal(userId, task.CreatedById);
        Assert.Equal(assigneeId, task.Assignment!.AssigneeId);
        Assert.Equal(userId, task.Assignment!.AssignedById);
    }

    [Fact]
    public void CreateTask_ThrowsOnAssignTaskWithNoDirection()
    {
        var startAt = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();

        var exception = Assert.Throws<DomainException>(() => new Task(
            description: Description.From("description"),
            details: Details.From("details"),
            createdById: userId,
            startAt: startAt,
            assignment: new Assignment(assigneeId, userId),
            directionId: null));

        Assert.Equivalent(TaskErrors.AssignPrivateTask, exception.Error);
    }

    [Theory]
    [InlineData(RepeatMode.Week, 1, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday)]
    [InlineData(RepeatMode.Month, 2, null)]
    [InlineData(RepeatMode.Day, 2, null)]
    [InlineData(RepeatMode.Year, 1, null)]
    public void CreateRecurringTask(RepeatMode repeatMode, int repeatEvery, WeekDay? weekDays)
    {
        var startAt = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var groupId = Guid.NewGuid();

        var task = new Task(
            description: Description.From("description"),
            details: Details.From("details"),
            createdById: userId,
            startAt: startAt,
            assignment: null,
            directionId: directionId,
            groupId: groupId,
            recurrencePattern: new RecurrencePattern(repeatMode, weekDays, repeatEvery, startAt.AddDays(7)));

        Assert.Equal(ActionItemType.Task, task.ActionItemType);
        Assert.NotNull(task.Description);
        Assert.NotNull(task.Details);
        Assert.NotNull(task.CreatedAt);
        Assert.NotNull(task.StartAt);
        Assert.Null(task.CompletedAt);
        Assert.Null(task.DeletedAt);
        Assert.NotNull(task.Assignment);
        Assert.NotNull(task.DirectionId);
        Assert.NotNull(task.GroupId);
        Assert.NotNull(task.RecurrencePattern);
        Assert.Equal(Description.From("description"), task.Description);
        Assert.Equal(Details.From("details"), task.Details);
        Assert.Equal(startAt, task.StartAt);
        Assert.Equal(userId, task.CreatedById);
        Assert.Equal(directionId, task.DirectionId);
        Assert.Equal(userId, task.Assignment!.AssigneeId);
        Assert.Equal(userId, task.Assignment!.AssignedById);
        Assert.Equivalent(new RecurrencePattern(repeatMode, weekDays, repeatEvery, startAt.AddDays(7)),
            task.RecurrencePattern);
    }

    [Theory]
    [InlineData(RepeatMode.Week, -1, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday)]
    [InlineData(RepeatMode.Day, -1, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday)]
    [InlineData(RepeatMode.Month, -1, null)]
    [InlineData(RepeatMode.Year, -1, null)]
    [InlineData(RepeatMode.Week, 0, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday)]
    [InlineData(RepeatMode.Day, 0, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday)]
    [InlineData(RepeatMode.Month, 0, null)]
    [InlineData(RepeatMode.Year, 0, null)]
    [InlineData(null, 1, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday)]
    [InlineData(RepeatMode.Day, null, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday)]
    public void CreateRecurringTask_ThrowsWhenInvalidRecurrencePattern(RepeatMode? repeatMode, int? repeatEvery,
        WeekDay? weekDays)
    {
        var startAt = DateTime.UtcNow;
        Assert.Throws<DomainException>(() => new Task(description: Description.From("description"),
            details: Details.From("details"),
            createdById: Guid.NewGuid(),
            startAt: startAt,
            assignment: null,
            directionId: null,
            groupId: Guid.NewGuid(),
            recurrencePattern: new RecurrencePattern(repeatMode, weekDays, repeatEvery, startAt.AddDays(7))));

        Assert.Throws<DomainException>(() => new Task(description: Description.From("description"),
            details: Details.From("details"),
            createdById: Guid.NewGuid(),
            startAt: startAt,
            assignment: null,
            directionId: null,
            groupId: Guid.NewGuid(),
            recurrencePattern: new RecurrencePattern(RepeatMode.Day, null, 1, startAt.AddDays(-7))));
    }

    [Fact]
    public void UpdateTaskDescription()
    {
        var task = GetValidTask();

        task.UpdateDescription(Description.From("new description"));

        Assert.Equal(Description.From("new description"), task.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateTaskDescription_ThrowsWhenInvalid(string? description)
    {
        var task = GetValidTask();

        Assert.Throws<DomainException>(() => task.UpdateDescription(Description.From(description!)));
    }

    [Fact]
    public void UpdateTaskDetails()
    {
        var task = GetValidTask();

        task.UpdateDetails(Details.From("new details"));

        Assert.Equal(Details.From("new details"), task.Details);
    }

    [Fact]
    public void UpdateTaskDetails_ThrowsWhenInvalid()
    {
        var task = GetValidTask();
        var tooLongDetails = new string('a', 100000);

        Assert.Throws<DomainException>(() => task.UpdateDetails(Details.From(tooLongDetails)));
    }

    [Fact]
    public void UpdateTaskStartAt()
    {
        var task = GetValidTask();
        var newStartAt = DateTime.UtcNow.AddDays(1);

        task.UpdateStartAt(newStartAt);

        Assert.Equal(newStartAt, task.StartAt);
    }

    [Fact]
    public void UpdateTaskStartAt_ThrowsWhenInvalidForRecurringTask()
    {
        var task = GetValidRecurringTask();
        var newStartAt = task.RecurrencePattern!.RepeatUntil.AddDays(1);

        Assert.Throws<DomainException>(() => task.UpdateStartAt(newStartAt));
    }

    [Fact]
    public void UpdateCompletedAt()
    {
        var task = GetValidTask();
        var completedAt = CompletedAt.From(DateTime.UtcNow);

        task.UpdateCompletedAt(completedAt);

        Assert.Equal(completedAt, task.CompletedAt);
    }

    [Fact]
    public void UpdateCompletedAt_ThrowsWhenInvalid()
    {
        Assert.Throws<DomainException>(() => CompletedAt.From(DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void UpdateCompletedAt_ThrowsWhenAlreadyCompleted()
    {
        var task = GetValidTask();
        var completedAt = CompletedAt.From(DateTime.UtcNow);

        task.UpdateCompletedAt(completedAt);

        Assert.Throws<DomainException>(() => task.UpdateCompletedAt(completedAt));
    }

    [Fact]
    public void UpdateDeletedAt()
    {
        var task = GetValidTask();
        var deletedAt = DeletedAt.From(DateTime.UtcNow);

        task.UpdateDeletedAt(deletedAt);

        Assert.Equal(deletedAt, task.DeletedAt);
    }

    [Fact]
    public void UpdateDeletedAt_ThrowsWhenInvalid()
    {
        Assert.Throws<DomainException>(() => DeletedAt.From(DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void UpdateDeletedAt_ThrowsWhenAlreadyDeleted()
    {
        var task = GetValidTask();
        var deletedAt = DeletedAt.From(DateTime.UtcNow);

        task.UpdateDeletedAt(deletedAt);

        Assert.Throws<DomainException>(() => task.UpdateDeletedAt(deletedAt));
    }

    [Fact]
    public void UpdateAssignment()
    {
        var task = GetValidTaskWithDirection();
        var assigneeId = Guid.NewGuid();

        task.UpdateAssignment(new Assignment(assigneeId, task.CreatedById));

        Assert.Equivalent(new Assignment(assigneeId, task.CreatedById), task.Assignment);
    }

    [Fact]
    public void UpdateAssignment_ThrowsWhenNoDirection()
    {
        var task = GetValidTask();
        var assigneeId = Guid.NewGuid();

        Assert.Throws<DomainException>(() => task.UpdateAssignment(new Assignment(assigneeId, task.CreatedById)));
    }

    [Fact]
    public void RemoveTaskFromGroup()
    {
        var task = GetValidRecurringTask();

        task.RemoveFromGroup();

        Assert.Null(task.GroupId);
        Assert.Null(task.RecurrencePattern);
    }

    [Fact]
    public void UpdateDeletedTask_Throws()
    {
        var task = GetValidTask();

        task.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow));

        Assert.Throws<DomainException>(() => task.UpdateDescription(Description.From("new description")));
        Assert.Throws<DomainException>(() => task.UpdateDetails(Details.From("new details")));
        Assert.Throws<DomainException>(() => task.UpdateStartAt(DateTime.UtcNow.AddDays(1)));
        Assert.Throws<DomainException>(() => task.UpdateCompletedAt(CompletedAt.From(DateTime.UtcNow)));
        Assert.Throws<DomainException>(() => task.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow)));
        Assert.Throws<DomainException>(() => task.UpdateAssignment(new Assignment(Guid.NewGuid(), Guid.NewGuid())));
        Assert.Throws<DomainException>(() => task.UpdateDirectionId(Guid.NewGuid()));

        // undelete
        task.UpdateDeletedAt(null);

        Assert.Null(task.DeletedAt);
    }

    [Fact]
    public void UpdateCompletedTask_Throws()
    {
        var task = GetValidTask();

        task.UpdateCompletedAt(CompletedAt.From(DateTime.UtcNow));

        Assert.Throws<DomainException>(() => task.UpdateDescription(Description.From("new description")));
        Assert.Throws<DomainException>(() => task.UpdateDetails(Details.From("new details")));
        Assert.Throws<DomainException>(() => task.UpdateStartAt(DateTime.UtcNow.AddDays(1)));
        Assert.Throws<DomainException>(() => task.UpdateCompletedAt(CompletedAt.From(DateTime.UtcNow)));
        Assert.Throws<DomainException>(() => task.UpdateAssignment(new Assignment(Guid.NewGuid(), Guid.NewGuid())));
        Assert.Throws<DomainException>(() => task.UpdateDirectionId(Guid.NewGuid()));

        task.UpdateCompletedAt(null);

        Assert.Null(task.CompletedAt);
    }

    [Fact]
    public void UpdateDirectionId()
    {
        var task = GetValidTask();
        var directionId = Guid.NewGuid();

        task.UpdateDirectionId(directionId);
        Assert.Equal(directionId, task.DirectionId);

        var newDirectionId = Guid.NewGuid();

        task.UpdateDirectionId(newDirectionId);
        Assert.Equal(newDirectionId, task.DirectionId);

        task.UpdateDirectionId(null);
        Assert.Null(task.DirectionId);
    }

    [Theory]
    [InlineData(RepeatMode.Week, 1, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday)]
    [InlineData(RepeatMode.Month, 2, null)]
    [InlineData(RepeatMode.Day, 2, null)]
    [InlineData(RepeatMode.Year, 1, null)]
    public void UpdateRecurrencePattern(RepeatMode? repeatMode, int? repeatEvery, WeekDay? weekDays)
    {
        var task = GetValidTask();
        var recurrencePattern = new RecurrencePattern(repeatMode, weekDays, repeatEvery, DateTime.UtcNow.AddDays(7));

        task.UpdateRecurrencePattern(recurrencePattern);

        Assert.Equal(recurrencePattern, task.RecurrencePattern);
    }

    [Theory]
    [InlineData(RepeatMode.Week, 1, null, "2023-03-20", "2023-05-20")]
    [InlineData(RepeatMode.Month, -2, null, "2023-03-20", "2023-05-20")]
    [InlineData(RepeatMode.Month, 0, null, "2023-03-20", "2023-05-20")]
    [InlineData(RepeatMode.Month, null, null, "2023-03-20", "2023-05-20")]
    [InlineData(RepeatMode.Day, 2, null, "2023-05-20", "2023-03-20")]
    [InlineData(RepeatMode.Year, 1, null, "2023-03-20", null)]
    public void UpdateRecurrencePattern_ThrowsWhenInvalid(RepeatMode? repeatMode, int? repeatEvery, WeekDay? weekDays,
        string startAt, string? repeatUntil)
    {
        var task = GetValidTask();
        task.UpdateStartAt(DateTime.Parse(startAt));

        DateTime? repeatUntilTime = repeatUntil == null ? null : DateTime.Parse(repeatUntil);

        Assert.Throws<DomainException>(() =>
            task.UpdateRecurrencePattern(new RecurrencePattern(repeatMode, weekDays, repeatEvery, repeatUntilTime)));
    }

    private Task GetValidTask()
    {
        return new Task(
            description: Description.From("description"),
            details: Details.From("details"),
            createdById: Guid.NewGuid(),
            startAt: DateTime.UtcNow,
            assignment: null,
            directionId: null);
    }

    private Task GetValidTaskWithDirection()
    {
        return new Task(
            description: Description.From("description"),
            details: Details.From("details"),
            createdById: Guid.NewGuid(),
            startAt: DateTime.UtcNow,
            assignment: null,
            directionId: Guid.NewGuid());
    }

    private Task GetValidRecurringTask()
    {
        return new Task(
            description: Description.From("description"),
            details: Details.From("details"),
            createdById: Guid.NewGuid(),
            startAt: DateTime.UtcNow,
            assignment: null,
            directionId: Guid.NewGuid(),
            groupId: Guid.NewGuid(),
            recurrencePattern: new RecurrencePattern(RepeatMode.Day, null, 1, DateTime.UtcNow.AddDays(7)));
    }
}