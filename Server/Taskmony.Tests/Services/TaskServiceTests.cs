using Moq;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Tasks;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;
using Taskmony.Services;
using Taskmony.Services.Abstract;
using Taskmony.Tests.Fixtures;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly Mock<IDirectionRepository> _mockDirectionRepository;
    private readonly Mock<IAssignmentRepository> _mockAssignmentRepository;
    private readonly Mock<ICommentRepository> _mockCommentRepository;
    private readonly TaskService _taskService;
    private readonly TimeConverter _timeConverter;

    public TaskServiceTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _mockDirectionRepository = new Mock<IDirectionRepository>();
        _mockAssignmentRepository = new Mock<IAssignmentRepository>();
        _timeConverter = new TimeConverter();
        _mockCommentRepository = new Mock<ICommentRepository>();

        _taskService = new TaskService(_mockTaskRepository.Object, _mockDirectionRepository.Object,
            _mockAssignmentRepository.Object, new Mock<INotificationService>().Object, _timeConverter,
            _mockCommentRepository.Object, new RecurringTaskGenerator());
    }

    [Fact]
    public async Task GetTasksAsync_ReturnsAllUserTasksWhenFiltersAreEmpty()
    {
        var userId = Guid.NewGuid();
        var tasks = TaskFixture.GetTasks(userId).ToList();
        var userDirectionIds = tasks.Where(i => i.DirectionId != null).Select(i => i.DirectionId).Distinct().ToList();
        var authorizedDirectionIds = userDirectionIds.Append(null);

        _mockDirectionRepository.Setup(r => r.GetUserDirectionIdsAsync(userId))
            .ReturnsAsync(userDirectionIds.Cast<Guid>());
        _mockTaskRepository
            .Setup(r => r.GetAsync(null, authorizedDirectionIds.ToArray(), null, null, null, null, null, null, userId))
            .ReturnsAsync(tasks);

        var result = await _taskService.GetTasksAsync(null, null, null, null, null, null, null, null, userId);

        Assert.Equal(tasks, result);

        _mockTaskRepository.Verify(
            r => r.GetAsync(null, authorizedDirectionIds.ToArray(), null, null, null, null, null, null, userId),
            Times.Once);
    }

    [Fact]
    public async Task AddTaskAsync()
    {
        var userId = Guid.NewGuid();
        var startAt = DateTime.UtcNow;

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _taskService.AddTaskAsync(
            description: "description",
            details: null,
            assigneeId: null,
            directionId: null,
            startAtUtc: startAt,
            currentUserId: userId);

        Assert.NotNull(result);
        Assert.NotNull(result.Description);
        Assert.Equal("description", result.Description.Value);
        Assert.NotNull(result.Details);
        Assert.Null(result.Details.Value);
        Assert.Null(result.DirectionId);
        Assert.Equal(userId, result.CreatedById);
        Assert.Equal(startAt, result.StartAt);
        Assert.Null(result.Assignment);

        _mockTaskRepository.Verify(r => r.AddAsync(result), Times.Once);
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddRecurringTaskAsync()
    {
        var userId = Guid.NewGuid();
        var repeatUntil = DateTime.UtcNow.AddDays(7);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _taskService.AddRecurringTaskAsync(
            description: "description",
            details: null,
            assigneeId: null,
            directionId: null,
            startAtUtc: DateTime.UtcNow,
            repeatMode: RepeatMode.Day,
            repeatEvery: 1,
            repeatUntilUtc: repeatUntil,
            weekDays: null,
            currentUserId: userId);

        Assert.NotNull(result);
        Assert.Equal(7, result.Count());

        _mockTaskRepository.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Models.Tasks.Task>>(
            instances => instances.All(t =>
                t.Description != null && t.Description.Value == "description" &&
                t.Details != null && t.Details.Value == null &&
                t.Assignment == null &&
                t.DirectionId == null &&
                t.StartAt != null &&
                t.CreatedById == userId &&
                t.RecurrencePattern != null &&
                t.RecurrencePattern.RepeatMode == RepeatMode.Day &&
                t.RecurrencePattern.RepeatEvery == 1 &&
                t.RecurrencePattern.RepeatUntil == repeatUntil &&
                t.RecurrencePattern.WeekDays == null
            ))), Times.Once);
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddTaskAsync_WithAssignment()
    {
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository
            .Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository
            .Setup(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var result = await _taskService.AddTaskAsync(
            description: "description",
            details: "details",
            assigneeId: assigneeId,
            directionId: direction.Id,
            startAtUtc: DateTime.UtcNow,
            currentUserId: userId);

        Assert.NotNull(result);
        Assert.NotNull(result.Description);
        Assert.Equal("description", result.Description.Value);
        Assert.NotNull(result.Details);
        Assert.Equal("details", result.Details.Value);
        Assert.NotNull(result.DirectionId);
        Assert.Equal(direction.Id, result.DirectionId!.Value);
        Assert.NotNull(result.Assignment);
        Assert.Equal(assigneeId, result.Assignment.AssigneeId);
        Assert.Equal(userId, result.Assignment.AssignedById);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId), Times.Once);
        _mockTaskRepository.Verify(r => r.AddAsync(result), Times.Once);
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddRecurringTaskAsync_WithAssignment()
    {
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var repeatUntil = DateTime.UtcNow.AddDays(7);
        var direction = DirectionFixture.GetUserDirection(userId);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository
            .Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository
            .Setup(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var result = await _taskService.AddRecurringTaskAsync(
            description: "description",
            details: null,
            assigneeId: assigneeId,
            directionId: direction.Id,
            startAtUtc: DateTime.UtcNow,
            repeatMode: RepeatMode.Day,
            repeatEvery: 1,
            repeatUntilUtc: repeatUntil,
            weekDays: null,
            currentUserId: userId);

        Assert.NotNull(result);
        Assert.Equal(7, result.Count());

        _mockTaskRepository.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Models.Tasks.Task>>(
            instances => instances.All(t =>
                t.Description != null && t.Description.Value == "description" &&
                t.Details != null && t.Details.Value == null &&
                t.Assignment != null && t.Assignment.AssigneeId == assigneeId && t.Assignment.AssignedById == userId &&
                t.DirectionId != null && t.DirectionId == direction.Id &&
                t.StartAt != null &&
                t.CreatedById == userId &&
                t.RecurrencePattern != null &&
                t.RecurrencePattern.RepeatMode == RepeatMode.Day &&
                t.RecurrencePattern.RepeatEvery == 1 &&
                t.RecurrencePattern.RepeatUntil == repeatUntil &&
                t.RecurrencePattern.WeekDays == null
            ))), Times.Once);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId), Times.Once);
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddTaskAsync_ThrowsWhenUserDoesNotHaveDirection()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(Guid.NewGuid());

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(false);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddTaskAsync(
            description: "description",
            details: null,
            assigneeId: null,
            directionId: direction.Id,
            startAtUtc: DateTime.UtcNow,
            currentUserId: userId));

        Assert.Equivalent(DirectionErrors.NotFound, exception.Error);

        _mockDirectionRepository
            .Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTaskRepository.Verify(r => r.AddAsync(It.IsAny<Models.Tasks.Task>()), Times.Never);
    }

    [Fact]
    public async Task AddRecurringTaskAsync_ThrowsWhenUserDoesNotHaveDirection()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(Guid.NewGuid());

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(false);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddRecurringTaskAsync(
            description: "description",
            details: null,
            assigneeId: null,
            directionId: direction.Id,
            startAtUtc: DateTime.UtcNow,
            repeatMode: RepeatMode.Day,
            repeatEvery: 1,
            repeatUntilUtc: DateTime.UtcNow.AddDays(7),
            weekDays: null,
            currentUserId: userId));

        Assert.Equivalent(DirectionErrors.NotFound, exception.Error);

        _mockDirectionRepository
            .Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTaskRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Models.Tasks.Task>>()), Times.Never);
    }

    [Fact]
    public async Task AddTaskAsync_SetsAssignmentIfDirectionIsPresent()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository
            .Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var result = await _taskService.AddTaskAsync(
            description: "description",
            details: null,
            assigneeId: null,
            directionId: direction.Id,
            startAtUtc: DateTime.UtcNow,
            currentUserId: userId);

        Assert.NotNull(result);
        Assert.NotNull(result.Assignment);
        Assert.Equal(userId, result.Assignment.AssigneeId);
        Assert.Equal(userId, result.Assignment.AssignedById);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId),
            Times.Exactly(2));
        _mockTaskRepository.Verify(r => r.AddAsync(result), Times.Once);
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddRecurringTaskAsync_SetsAssignmentIfDirectionIsPresent()
    {
        var userId = Guid.NewGuid();
        var repeatUntil = DateTime.UtcNow.AddDays(7);
        var direction = DirectionFixture.GetUserDirection(userId);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository
            .Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var result = await _taskService.AddRecurringTaskAsync(
            description: "description",
            details: null,
            assigneeId: null,
            directionId: direction.Id,
            startAtUtc: DateTime.UtcNow,
            repeatMode: RepeatMode.Day,
            repeatEvery: 1,
            repeatUntilUtc: repeatUntil,
            weekDays: null,
            currentUserId: userId);

        Assert.NotNull(result);
        Assert.Equal(7, result.Count());

        _mockTaskRepository.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Models.Tasks.Task>>(
            instances => instances.All(t =>
                t.Description != null && t.Description.Value == "description" &&
                t.Details != null && t.Details.Value == null &&
                t.Assignment != null && t.Assignment.AssigneeId == userId && t.Assignment.AssignedById == userId &&
                t.DirectionId != null && t.DirectionId == direction.Id &&
                t.StartAt != null &&
                t.CreatedById == userId &&
                t.RecurrencePattern != null &&
                t.RecurrencePattern.RepeatMode == RepeatMode.Day &&
                t.RecurrencePattern.RepeatEvery == 1 &&
                t.RecurrencePattern.RepeatUntil == repeatUntil &&
                t.RecurrencePattern.WeekDays == null
            ))), Times.Once);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Exactly(2));
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddTaskAsync_ThrowsWhenAssigneeNotFound()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        var assigneeId = Guid.NewGuid();

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId))
            .ReturnsAsync(false);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddTaskAsync(
            description: "description",
            details: "details",
            assigneeId: assigneeId,
            directionId: direction.Id,
            startAtUtc: DateTime.UtcNow,
            currentUserId: userId));

        Assert.Equivalent(DirectionErrors.MemberNotFound, exception.Error);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId),
            Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId),
            Times.Once);
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTaskRepository.Verify(r => r.AddAsync(It.IsAny<Models.Tasks.Task>()), Times.Never);
    }

    [Fact]
    public async Task AddRecurringTaskAsync_ThrowsWhenAssigneeNotFound()
    {
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository
            .Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository
            .Setup(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId))
            .ReturnsAsync(false);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddRecurringTaskAsync(
            description: "description",
            details: null,
            assigneeId: assigneeId,
            directionId: direction.Id,
            startAtUtc: DateTime.UtcNow,
            repeatMode: RepeatMode.Day,
            repeatEvery: 1,
            repeatUntilUtc: DateTime.UtcNow.AddDays(7),
            weekDays: null,
            currentUserId: userId));

        Assert.Equivalent(DirectionErrors.MemberNotFound, exception.Error);

        _mockTaskRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Models.Tasks.Task>>()), Times.Never);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId), Times.Once);
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetTaskOrThrowAsync_PrivateTaskCreatedByUser()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);

        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.GetTaskOrThrowAsync(task.Id, userId);

        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);

        _mockTaskRepository.Verify(r => r.GetByIdAsync(task.Id), Times.Once);
    }

    [Fact]
    public async Task GetTaskOrThrowAsync_TaskFromUserDirection()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        var task = TaskFixture.GetTask(userId, direction.Id);

        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);

        var result = await _taskService.GetTaskOrThrowAsync(task.Id, userId);

        Assert.NotNull(result);
        Assert.Equal(task.Id, result.Id);

        _mockTaskRepository.Verify(r => r.GetByIdAsync(task.Id), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId),
            Times.Once);
    }

    [Fact]
    public async Task GetTaskOrThrowAsync_ThrowsOnPrivateTaskCreatedByOtherUser()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(Guid.NewGuid());

        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _taskService.GetTaskOrThrowAsync(task.Id, userId));

        Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);

        _mockTaskRepository.Verify(r => r.GetByIdAsync(task.Id), Times.Once);
    }

    [Fact]
    public async Task GetTaskOrThrowAsync_TaskFromInaccessibleDirection()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        var task = TaskFixture.GetTask(userId, direction.Id);

        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _taskService.GetTaskOrThrowAsync(task.Id, userId));

        Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);

        _mockTaskRepository.Verify(r => r.GetByIdAsync(task.Id), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId),
            Times.Once);
    }

    [Fact]
    public async Task SetTaskDescriptionAsync()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.SetTaskDescriptionAsync(task.Id, "new description", userId);

        Assert.NotNull(result);
        Assert.NotNull(task.Description);
        Assert.Equal("new description", task.Description.Value);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetRecurringTaskDescriptionAsync()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetDailyRecurringTask(userId);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetActiveTasksAsync(task.GroupId!.Value)).ReturnsAsync(new[] {task});

        var result = await _taskService.SetRecurringTaskDescriptionAsync(task.Id, task.GroupId!.Value,
            "new description", userId);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.NotNull(task.Description);
        Assert.Equal("new description", task.Description.Value);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetTaskDetailsAsync()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.SetTaskDetailsAsync(task.Id, "new details", userId);

        Assert.NotNull(result);
        Assert.NotNull(task.Details);
        Assert.Equal("new details", task.Details.Value);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetRecurringTaskDetailsAsync()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetDailyRecurringTask(userId);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetActiveTasksAsync(task.GroupId!.Value)).ReturnsAsync(new[] {task});

        var result = await _taskService.SetRecurringTaskDetailsAsync(task.Id, task.GroupId!.Value,
            "new details", userId);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.NotNull(task.Details);
        Assert.Equal("new details", task.Details.Value);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetTaskStartAt()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var startAt = DateTime.UtcNow;

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.SetTaskStartAtAsync(task.Id, startAt, userId);

        Assert.NotNull(result);
        Assert.NotNull(task.StartAt);
        Assert.Equal(startAt, task.StartAt.Value);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetTaskCompletedAt()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var completedAt = DateTime.UtcNow;

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.SetTaskCompletedAtAsync(task.Id, completedAt, userId);

        Assert.NotNull(result);
        Assert.NotNull(task.CompletedAt);
        Assert.Equal(completedAt, task.CompletedAt.Value);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetTaskDeletedAtAsync_CallsSoftDeleteComments()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var deletedAt = DateTime.UtcNow;

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.SetTaskDeletedAtAsync(task.Id, deletedAt, userId);

        Assert.NotNull(result);
        Assert.NotNull(task.DeletedAt);
        Assert.Equal(deletedAt, task.DeletedAt.Value);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockCommentRepository.Verify(r => r.SoftDeleteTaskCommentsAsync(new[] {task.Id}), Times.Once);
    }

    [Fact]
    public async Task SetTaskDeletedAtAsync_CallsUndeleteCommentsWhenSetToNull()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var deletedAt = DateTime.UtcNow;
        task.UpdateDeletedAt(DeletedAt.From(deletedAt));

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.SetTaskDeletedAtAsync(task.Id, null, userId);

        Assert.NotNull(result);
        Assert.Null(task.DeletedAt);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockCommentRepository.Verify(r => r.UndeleteTaskCommentsAsync(new[] {task.Id}, deletedAt), Times.Once);
    }
    
    [Fact]
    public async Task SetTaskDeletedAtAsync_ThrowsWhenAlreadyDeleted()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        task.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow));

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _taskService.SetTaskDeletedAtAsync(task.Id, DateTime.UtcNow, userId));

        Assert.Equivalent(TaskErrors.AlreadyDeleted, exception.Error);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockCommentRepository.Verify(r => r.SoftDeleteTaskCommentsAsync(It.IsAny<IEnumerable<Guid>>()), Times.Never);
    }

    [Fact]
    public async Task SetTaskDirectionAsync_WhenOldIsNull()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var direction = DirectionFixture.GetUserDirection(userId);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var result = await _taskService.SetTaskDirectionAsync(task.Id, direction.Id, userId);

        Assert.NotNull(result);
        Assert.NotNull(task.DirectionId);
        Assert.Equal(direction.Id, task.DirectionId);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.GetByIdAsync(direction.Id), Times.Once);
    }

    [Fact]
    public async Task SetTaskDirectionAsync_WhenOldIsNotNull()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var direction = DirectionFixture.GetUserDirection(userId);
        var oldDirection = DirectionFixture.GetUserDirection(userId);
        task.UpdateDirectionId(oldDirection.Id);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(oldDirection.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var result = await _taskService.SetTaskDirectionAsync(task.Id, direction.Id, userId);

        Assert.NotNull(result);
        Assert.NotNull(task.DirectionId);
        Assert.Equal(direction.Id, task.DirectionId);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.GetByIdAsync(direction.Id), Times.Once);
    }

    [Fact]
    public async Task SetTaskDirectionAsync_WhenNewIsNull()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var oldDirection = DirectionFixture.GetUserDirection(userId);
        task.UpdateDirectionId(oldDirection.Id);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(oldDirection.Id, userId))
            .ReturnsAsync(true);

        var result = await _taskService.SetTaskDirectionAsync(task.Id, null, userId);

        Assert.NotNull(result);
        Assert.Null(task.DirectionId);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(oldDirection.Id, userId), Times.Once);
    }

    [Fact]
    public async Task SetTaskDirectionAsync_ThrowsWhenDirectionIsDeleted()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        var task = TaskFixture.GetTask(userId);
        direction.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow));

        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _taskService.SetTaskDirectionAsync(task.Id, direction.Id, userId));

        Assert.Equivalent(DirectionErrors.UpdateDeletedDirection, exception.Error);

        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task SetTaskAssignee()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var assigneeId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        task.UpdateDirectionId(direction.Id);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId))
            .ReturnsAsync(true);
        _mockAssignmentRepository.Setup(r =>
                r.UpdateAssignmentAsync(task,
                    It.Is<Assignment>(a => a.AssigneeId == assigneeId && a.AssignedById == userId)))
            .ReturnsAsync(true);

        var result = await _taskService.SetTaskAssigneeAsync(task.Id, assigneeId, userId);

        Assert.NotNull(result);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId), Times.Once);
        _mockAssignmentRepository.Verify(r =>
            r.UpdateAssignmentAsync(task,
                It.Is<Assignment>(a => a.AssigneeId == assigneeId && a.AssignedById == userId)), Times.Once);
    }

    [Fact]
    public async Task SetTaskAssignee_ToNull()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var direction = DirectionFixture.GetUserDirection(userId);
        task.UpdateDirectionId(direction.Id);
        task.UpdateAssignment(new Assignment(userId, userId));

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockAssignmentRepository.Setup(r => r.UpdateAssignmentAsync(task, null))
            .ReturnsAsync(true);

        var result = await _taskService.SetTaskAssigneeAsync(task.Id, null, userId);

        Assert.NotNull(result);

        _mockAssignmentRepository.Verify(r => r.UpdateAssignmentAsync(task, null), Times.Once);
    }

    [Fact]
    public async Task SetTaskAssignee_WhenAssigneeIsNotMember()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var assigneeId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        task.UpdateDirectionId(direction.Id);

        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId))
            .ReturnsAsync(false);

        var exception =
            await Assert.ThrowsAsync<DomainException>(() =>
                _taskService.SetTaskAssigneeAsync(task.Id, assigneeId, userId));

        Assert.Equivalent(DirectionErrors.MemberNotFound, exception.Error);
        Assert.Null(task.Assignment);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, assigneeId), Times.Once);
        _mockAssignmentRepository.Verify(r => r.UpdateAssignmentAsync(task, null), Times.Never);
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData(RepeatMode.Day, 1, null, "2023-03-10", "2023-03-20", 11)]
    [InlineData(RepeatMode.Day, 1, null, "2023-01-01", "2023-12-31", 365)]
    [InlineData(RepeatMode.Day, 1, null, "2024-01-01", "2024-12-31", 366)]
    [InlineData(RepeatMode.Day, 1, null, "2023-03-01", "2023-03-31", 31)]
    [InlineData(RepeatMode.Day, 7, null, "2023-03-06", "2023-03-27", 4)]
    [InlineData(RepeatMode.Day, 7, null, "2023-03-07", "2023-03-27", 3)]
    [InlineData(RepeatMode.Day, 7, null, "2023-03-06", "2023-03-26", 3)]
    [InlineData(RepeatMode.Day, 1, WeekDay.Monday, "2023-03-10", "2023-03-20", 11)]
    [InlineData(RepeatMode.Week, 1, WeekDay.Monday, "2023-03-10", "2023-03-24", 2)]
    [InlineData(RepeatMode.Week, 1, WeekDay.Monday, "2023-03-13", "2023-03-20", 2)]
    [InlineData(RepeatMode.Week, 1, WeekDay.Monday, "2023-03-14", "2023-03-20", 1)]
    [InlineData(RepeatMode.Week, 2, WeekDay.Monday, "2023-03-10", "2023-03-20", 1)]
    [InlineData(RepeatMode.Week, 1, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday, "2023-03-10", "2023-03-20", 5)]
    [InlineData(RepeatMode.Week, 1, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday, "2023-03-10", "2023-03-19", 4)]
    [InlineData(RepeatMode.Week, 1, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday, "2023-03-11", "2023-03-20", 4)]
    [InlineData(RepeatMode.Week, 2, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday, "2023-03-10", "2023-03-20", 2)]
    [InlineData(RepeatMode.Week, 2, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday, "2023-03-11", "2023-03-20", 1)]
    [InlineData(RepeatMode.Week, 2, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday, "2023-03-10", "2023-03-19", 1)]
    [InlineData(RepeatMode.Week, 2, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday, "2023-03-01", "2023-03-31", 8)]
    [InlineData(RepeatMode.Week, 2, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday, "2023-03-02", "2023-03-31", 7)]
    [InlineData(RepeatMode.Week, 2, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday, "2023-03-02", "2023-03-30", 6)]
    [InlineData(RepeatMode.Week, 4, WeekDay.Monday | WeekDay.Wednesday | WeekDay.Friday, "2023-03-01", "2023-04-30", 8)]
    [InlineData(RepeatMode.Month, 1, null, "2023-03-10", "2023-03-20", 1)]
    [InlineData(RepeatMode.Month, 2, null, "2023-03-10", "2023-06-10", 2)]
    [InlineData(RepeatMode.Month, 2, null, "2023-03-10", "2023-06-09", 2)]
    [InlineData(RepeatMode.Month, 2, null, "2023-03-10", "2023-06-11", 2)]
    [InlineData(RepeatMode.Year, 1, null, "2023-03-10", "2025-03-10", 3)]
    [InlineData(RepeatMode.Year, 2, null, "2023-03-10", "2025-03-10", 2)]
    [InlineData(RepeatMode.Year, 2, null, "2023-03-10", "2026-03-10", 2)]
    public async Task AddRecurringTask_ValidRange(RepeatMode repeatMode, int repeatEvery, WeekDay? weekDay,
        string startAt, string repeatUntil, int expectedNumberOfTasks)
    {
        var startAtUtc = _timeConverter.StringToDateTimeUtc(startAt);
        var repeatUntilUtc = _timeConverter.StringToDateTimeUtc(repeatUntil);
        _mockTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _taskService.AddRecurringTaskAsync(description: "description",
            details: null,
            assigneeId: null,
            directionId: null,
            startAtUtc: startAtUtc,
            repeatMode: repeatMode,
            repeatEvery: repeatEvery,
            repeatUntilUtc: repeatUntilUtc,
            weekDays: weekDay,
            currentUserId: Guid.NewGuid());

        Assert.Equal(expectedNumberOfTasks, result.Count());

        _mockTaskRepository.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Models.Tasks.Task>>(
            instances => instances.All(t => t.StartAt >= startAtUtc && t.StartAt <= repeatUntilUtc))), Times.Once);
        _mockTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}