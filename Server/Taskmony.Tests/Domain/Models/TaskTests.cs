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
    public void CreateTaskWithAssignment()
    {
        var startAt = DateTime.UtcNow;
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();

        var task = new Task(
            description: Description.From("description"),
            details: Details.From("details"),
            createdById: userId,
            startAt: startAt,
            assignment: new Assignment(assigneeId, userId),
            directionId: null);

        Assert.Equal(ActionItemType.Task, task.ActionItemType);
        Assert.NotNull(task.Description);
        Assert.NotNull(task.Details);
        Assert.NotNull(task.CreatedAt);
        Assert.NotNull(task.StartAt);
        Assert.Null(task.CompletedAt);
        Assert.Null(task.DeletedAt);
        Assert.NotNull(task.Assignment);
        Assert.Null(task.DirectionId);
        Assert.Null(task.GroupId);
        Assert.Equal(Description.From("description"), task.Description);
        Assert.Equal(Details.From("details"), task.Details);
        Assert.Equal(startAt, task.StartAt);
        Assert.Equal(userId, task.CreatedById);
        Assert.Equal(assigneeId, task.Assignment!.AssigneeId);
        Assert.Equal(userId, task.Assignment!.AssignedById);
    }

    [Fact]
    public void CreateRecurringTask()
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
            recurrencePattern: new RecurrencePattern(RepeatMode.Day, null, 1, startAt.AddDays(7)));

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
        Assert.Equivalent(new RecurrencePattern(RepeatMode.Day, null, 1, startAt.AddDays(7)), task.RecurrencePattern);
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