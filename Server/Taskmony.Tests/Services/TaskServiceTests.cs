using Moq;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.Repositories.Abstract;
using Taskmony.Services;
using Taskmony.Services.Abstract;
using Taskmony.Tests.Fixtures;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTasksRepository;
    private readonly Mock<IDirectionRepository> _mockDirectionRepository;
    private readonly Mock<IAssignmentRepository> _mockAssignmentRepository;
    private readonly TaskService _taskService;
    private readonly TimeConverter _timeConverter;

    public TaskServiceTests()
    {
        _mockTasksRepository = new Mock<ITaskRepository>();
        _mockDirectionRepository = new Mock<IDirectionRepository>();
        _mockAssignmentRepository = new Mock<IAssignmentRepository>();
        _timeConverter = new TimeConverter();

        _taskService = new TaskService(_mockTasksRepository.Object, _mockDirectionRepository.Object,
            _mockAssignmentRepository.Object, new Mock<INotificationService>().Object, _timeConverter);
    }

    [Fact]
    public async Task GetTasksAsync_ReturnsAllUserTasksWhenFiltersAreEmpty()
    {
        var userId = Guid.NewGuid();

        _mockTasksRepository
            .Setup(r => r.GetAsync(It.IsAny<Guid[]>(), It.IsAny<Guid?[]>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Guid>())).ReturnsAsync(TaskFixture.GetTasks(userId).Where(t => IsVisibleToUser(t, userId)));

        var result = await _taskService.GetTasksAsync(null, null, null, null, userId);

        Assert.IsAssignableFrom<List<Models.Task>>(result);
        Assert.All(result, t => Assert.True(IsVisibleToUser(t, userId)));
    }

    [Fact]
    public async Task GetTaskOrThrowAsync_ReturnsTaskCreatedByUser()
    {
        var userId = Guid.NewGuid();
        var tasks = TaskFixture.GetTasks(userId);
        var task = tasks.First(t => t.CreatedById == userId && t.Direction is null);

        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.GetTaskOrThrowAsync(task.Id, userId);

        Assert.Equal(task, result);
    }

    [Fact]
    public async Task GetTaskOrThrowAsync_ReturnsTaskFromUserDirection()
    {
        var userId = Guid.NewGuid();
        var tasks = TaskFixture.GetTasks(userId);
        var task = tasks.First(t =>
            t.Direction is not null && t.Direction.CreatedById != userId &&
            t.Direction.Members!.Any(m => m.Id == userId));

        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, userId)).ReturnsAsync(true);

        var result = await _taskService.GetTaskOrThrowAsync(task.Id, userId);

        Assert.Equal(task, result);
    }

    [Fact]
    public async Task GetTaskOrThrowAsync_ReturnsTaskFromDirectionCreatedByUser()
    {
        var userId = Guid.NewGuid();
        var tasks = TaskFixture.GetTasks(userId);
        var task = tasks.First(t =>
            t.Direction is not null && t.Direction.CreatedById == userId &&
            t.Direction.Members!.Any(m => m.Id == userId));

        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, userId)).ReturnsAsync(true);

        var result = await _taskService.GetTaskOrThrowAsync(task.Id, userId);

        Assert.Equal(task, result);
    }

    [Fact]
    public async Task GetTaskOrThrowAsync_ThrowsWhenTaskIsPrivateAndCreatedByOtherUser()
    {
        var userId = Guid.NewGuid();
        var tasks = TaskFixture.GetTasks(userId);
        var task = tasks.First(t => t.CreatedById != userId && t.Direction is null);

        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var exception =
            await Assert.ThrowsAsync<DomainException>(() => _taskService.GetTaskOrThrowAsync(task.Id, userId));
        Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);
    }

    [Fact]
    public async Task GetTaskOrThrowAsync_ThrowsWhenTaskIsFromInaccessibleDirection()
    {
        var userId = Guid.NewGuid();
        var tasks = TaskFixture.GetTasks(userId);
        var task = tasks.First(t => t.Direction is not null && t.Direction.Members!.All(m => m.Id != userId));

        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository
            .Setup(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, userId))
            .ReturnsAsync(false);

        var exception =
            await Assert.ThrowsAsync<DomainException>(() => _taskService.GetTaskOrThrowAsync(task.Id, userId));

        Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);
    }

    [Fact]
    public async Task AddTaskAsync_AddsAndReturnsTask()
    {
        var task = TaskFixture.GetTask(Guid.NewGuid());

        _mockTasksRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _taskService.AddTaskAsync(task);

        Assert.NotNull(result);
        Assert.Equal(task, result);

        _mockTasksRepository.Verify(r => r.AddAsync(task), Times.Once);
        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddRecurringTaskAsync_AddsTasksAndReturnsIds()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetDailyRecurringTask(userId);

        _mockTasksRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = (await _taskService.AddRecurringTaskAsync(task, task.RepeatMode!.Value,
            task.RepeatEvery!.Value, null, task.RepeatUntil!.Value)).ToList();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal((task.RepeatUntil!.Value - task.StartAt!.Value).Days + 1, result.Count);
        Assert.NotNull(task.GroupId);

        _mockTasksRepository
            .Verify(r => r.AddRangeAsync(
                It.Is<IEnumerable<Models.Task>>(t => t.All(x => x.GroupId == task.GroupId))), Times.Once);
        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddTaskAsync_ThrowsWhenUserDoesNotHaveDirection()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, userId))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddTaskAsync(task));

        Assert.Equivalent(DirectionErrors.NotFound, exception.Error);

        _mockDirectionRepository
            .Verify(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, task.CreatedById), Times.Once);
        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.AddAsync(It.IsAny<Models.Task>()), Times.Never);
    }

    [Fact]
    public async Task AddRecurringTaskAsync_ThrowsWhenUserDoesNotHaveDirection()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetDailyRecurringTask(userId, directionId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, userId))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddRecurringTaskAsync(task,
            task.RepeatMode!.Value, task.RepeatEvery!.Value, task.WeekDays, task.RepeatUntil!.Value));

        Assert.Equivalent(DirectionErrors.NotFound, exception.Error);

        _mockDirectionRepository
            .Verify(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, task.CreatedById), Times.Once);
        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Models.Task>>()), Times.Never);
    }

    [Fact]
    public async Task AddTaskAsync_SetsAssignmentIfDirectionIsPresent()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId);

        _mockTasksRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository
            .Setup(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, task.CreatedById))
            .ReturnsAsync(true);

        var result = await _taskService.AddTaskAsync(task);

        Assert.NotNull(result);
        Assert.NotNull(result.Assignment);
        Assert.Equal(userId, result.Assignment.AssigneeId);
        Assert.Equal(userId, result.Assignment.AssignedById);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, task.CreatedById),
            Times.Once);
        _mockTasksRepository.Verify(r => r.AddAsync(task), Times.Once);
        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddRecurringTaskAsync_SetsAssignmentIfDirectionIsPresent()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetDailyRecurringTask(userId, directionId);

        _mockTasksRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository
            .Setup(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, task.CreatedById))
            .ReturnsAsync(true);

        var result = await _taskService.AddRecurringTaskAsync(task, task.RepeatMode!.Value, task.RepeatEvery!.Value,
            task.WeekDays, task.RepeatUntil!.Value);

        Assert.NotNull(result);
        Assert.NotNull(task.Assignment);
        Assert.Equal(userId, task.Assignment.AssigneeId);
        Assert.Equal(userId, task.Assignment.AssignedById);

        _mockDirectionRepository
            .Verify(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, task.CreatedById), Times.Once);
        _mockTasksRepository
            .Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Models.Task>>(
                    t => t.All(x =>
                        x.Assignment != null && x.Assignment.AssigneeId == userId &&
                        x.Assignment.AssignedById == userId))),
                Times.Once);
        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddTaskAsync_SetsAssignerWhenAssigneeIsPresent()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId, assigneeId);

        _mockTasksRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, task.CreatedById))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, assigneeId))
            .ReturnsAsync(true);

        var result = await _taskService.AddTaskAsync(task);

        Assert.NotNull(result);
        Assert.NotNull(result.Assignment);
        Assert.Equal(assigneeId, result.Assignment.AssigneeId);
        Assert.Equal(userId, result.Assignment.AssignedById);
        Assert.Equal(directionId, result.DirectionId);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, task.CreatedById),
            Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, assigneeId),
            Times.Once);
        _mockTasksRepository.Verify(r => r.AddAsync(task), Times.Once);
        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddRecurringTaskAsync_SetsAssignerWhenAssigneeIsPresent()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var task = TaskFixture.GetDailyRecurringTask(userId, directionId, assigneeId);

        _mockTasksRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, task.CreatedById))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, assigneeId))
            .ReturnsAsync(true);

        var result = await _taskService.AddRecurringTaskAsync(task, task.RepeatMode!.Value, task.RepeatEvery!.Value,
            task.WeekDays, task.RepeatUntil!.Value);

        Assert.NotNull(result);
        Assert.NotNull(task.Assignment);
        Assert.Equal(assigneeId, task.Assignment.AssigneeId);
        Assert.Equal(userId, task.Assignment.AssignedById);
        Assert.Equal(directionId, task.DirectionId);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, task.CreatedById),
            Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, assigneeId),
            Times.Once);
        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockTasksRepository.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Models.Task>>(
            t => t.All(x =>
                x.Assignment != null && x.Assignment.AssigneeId == assigneeId && x.Assignment.AssignedById == userId &&
                x.DirectionId == directionId)
        )), Times.Once);
    }

    [Fact]
    public async Task AddTaskAsync_ThrowsWhenAssigneeNotFound()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId, assigneeId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, assigneeId))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddTaskAsync(task));

        Assert.Equivalent(DirectionErrors.MemberNotFound, exception.Error);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(directionId, userId),
            Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(directionId, assigneeId),
            Times.Once);
        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.AddAsync(It.IsAny<Models.Task>()), Times.Never);
    }

    [Fact]
    public async Task AddRecurringTaskAsync_ThrowsWhenAssigneeNotFound()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var task = TaskFixture.GetDailyRecurringTask(userId, directionId, assigneeId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, assigneeId))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddRecurringTaskAsync(task,
            task.RepeatMode!.Value, task.RepeatEvery!.Value, task.WeekDays, task.RepeatUntil!.Value));

        Assert.Equivalent(DirectionErrors.MemberNotFound, exception.Error);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(directionId, assigneeId),
            Times.Once);
        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Models.Task>>()), Times.Never);
    }

    [Fact]
    public async Task AddTaskAsync_ThrowsOnAssignTaskWithNoDirection()
    {
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, null, assigneeId);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddTaskAsync(task));

        Assert.Equivalent(TaskErrors.AssignPrivateTask, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.AddAsync(It.IsAny<Models.Task>()), Times.Never);
    }

    [Fact]
    public async Task AddRecurringTaskAsync_ThrowsOnAssignTaskWithNoDirection()
    {
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var task = TaskFixture.GetDailyRecurringTask(userId, null, assigneeId);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddRecurringTaskAsync(task,
            task.RepeatMode!.Value, task.RepeatEvery!.Value, task.WeekDays, task.RepeatUntil!.Value));

        Assert.Equivalent(TaskErrors.AssignPrivateTask, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Models.Task>>()), Times.Never);
    }

    [Fact]
    public async Task AddRecurringTask_ThrowsWhenWeekDaysAreNull()
    {
        var task = TaskFixture.GetRecurringTask(Guid.NewGuid(), RepeatMode.Week, 1, DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(7), null);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddRecurringTaskAsync(task,
            task.RepeatMode!.Value, task.RepeatEvery!.Value, task.WeekDays, task.RepeatUntil!.Value));

        Assert.Equivalent(ValidationErrors.WeekDaysAreRequired, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Models.Task>>()), Times.Never);
    }

    [Fact]
    public async Task AddRecurringTask_ThrowsWhenRepeatUntilIsBeforeStartAt()
    {
        var task = TaskFixture.GetRecurringTask(Guid.NewGuid(), RepeatMode.Day, 1, DateTime.UtcNow.Date.AddDays(7),
            DateTime.UtcNow.Date, null);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddRecurringTaskAsync(task,
            task.RepeatMode!.Value, task.RepeatEvery!.Value, task.WeekDays, task.RepeatUntil!.Value));

        Assert.Equivalent(ValidationErrors.RepeatUntilIsBeforeStartAt, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Models.Task>>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task AddRecurringTask_ThrowsWhenRepeatEveryIsNotPositive(int repeatEvery)
    {
        var task = TaskFixture.GetRecurringTask(Guid.NewGuid(), RepeatMode.Day, repeatEvery,
            DateTime.UtcNow.Date.AddDays(7),
            DateTime.UtcNow.Date, null);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.AddRecurringTaskAsync(task,
            task.RepeatMode!.Value, task.RepeatEvery!.Value, task.WeekDays, task.RepeatUntil!.Value));

        Assert.Equivalent(ValidationErrors.InvalidRepeatEvery, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Models.Task>>()), Times.Never);
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
    public async Task AddRecurringTask_ValidNumberOfTasks(RepeatMode repeatMode, int repeatEvery, WeekDay? weekDay,
        string startAt, string repeatUntil, int expectedNumberOfTasks)
    {
        var task = TaskFixture.GetRecurringTask(Guid.NewGuid(), repeatMode, repeatEvery,
            _timeConverter.StringToDateTimeUtc(startAt),
            _timeConverter.StringToDateTimeUtc(repeatUntil), weekDay);

        _mockTasksRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _taskService.AddRecurringTaskAsync(task, repeatMode, repeatEvery, weekDay,
            _timeConverter.StringToDateTimeUtc(repeatUntil));

        Assert.Equal(expectedNumberOfTasks, result.Count());

        _mockTasksRepository.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<Models.Task>>(
            t => t.Count() == expectedNumberOfTasks
        )), Times.Once);
        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetTaskAssigneeAsync_WhenOldAssigneeIsNull()
    {
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, userId)).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, assigneeId)).ReturnsAsync(true);
        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.SetTaskAssigneeAsync(task.Id, assigneeId, userId);

        Assert.NotNull(result);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockAssignmentRepository.Verify(r => r.UpdateAssignmentAsync(task, It.Is<Assignment>(
            a => a.AssigneeId == assigneeId && a.AssignedById == userId
        )), Times.Once);
    }

    [Fact]
    public async Task SetTaskAssigneeAsync_ReturnsNullWhenSameAssignee()
    {
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId, assigneeId, userId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, userId)).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, assigneeId)).ReturnsAsync(true);
        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.SetTaskAssigneeAsync(task.Id, assigneeId, userId);

        Assert.Null(result);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockAssignmentRepository.Verify(r => r.UpdateAssignmentAsync(task, It.IsAny<Assignment>()), Times.Never);
    }

    [Fact]
    public async Task SetTaskAssigneeAsync_WhenOldAssigneeIsNotNull()
    {
        var userId = Guid.NewGuid();
        var oldAssigneeId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId, oldAssigneeId, userId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, userId)).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, assigneeId)).ReturnsAsync(true);
        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.SetTaskAssigneeAsync(task.Id, assigneeId, userId);

        Assert.NotNull(result);

        _mockAssignmentRepository.Verify(r => r.UpdateAssignmentAsync(task, It.Is<Assignment>(
            a => a.AssigneeId == assigneeId && a.AssignedById == userId
        )), Times.Once);
    }

    [Fact]
    public async Task SetTaskAssigneeAsync_SetToNull()
    {
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId, assigneeId, userId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, userId)).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, assigneeId)).ReturnsAsync(true);
        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var result = await _taskService.SetTaskAssigneeAsync(task.Id, null, userId);

        Assert.NotNull(result);

        _mockAssignmentRepository.Verify(r => r.UpdateAssignmentAsync(task, It.Is<Assignment?>(a => a == null)),
            Times.Once);
    }

    [Fact]
    public async Task SetTaskAssigneeAsync_ThrowsWhenUserIsNotMemberOfDirection()
    {
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, userId)).ReturnsAsync(false);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, assigneeId)).ReturnsAsync(true);
        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var exception =
            await Assert.ThrowsAsync<DomainException>(() =>
                _taskService.SetTaskAssigneeAsync(task.Id, assigneeId, userId));

        Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);

        _mockAssignmentRepository.Verify(r => r.UpdateAssignmentAsync(task, It.IsAny<Assignment>()), Times.Never);
    }

    [Fact]
    public async Task SetTaskAssigneeAsync_ThrowsWhenAssigneeIsNotMemberOfDirection()
    {
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, userId)).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(directionId, assigneeId)).ReturnsAsync(false);
        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var exception =
            await Assert.ThrowsAsync<DomainException>(() =>
                _taskService.SetTaskAssigneeAsync(task.Id, assigneeId, userId));

        Assert.Equivalent(DirectionErrors.MemberNotFound, exception.Error);

        _mockAssignmentRepository.Verify(r => r.UpdateAssignmentAsync(task, It.IsAny<Assignment>()), Times.Never);
    }

    [Fact]
    public async Task SetTaskAssigneeAsync_ThrowsOnTaskWithNoDirection()
    {
        var userId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);

        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var exception =
            await Assert.ThrowsAsync<DomainException>(() =>
                _taskService.SetTaskAssigneeAsync(task.Id, assigneeId, userId));

        Assert.Equivalent(TaskErrors.AssignPrivateTask, exception.Error);
    }

    [Fact]
    public async Task SetTaskDescriptionAsync_OnTaskCreatedByUser()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var description = "New description";

        await TestTaskSetterAsync<string>(task, userId, description, t => t.Description!.Value,
            _taskService.SetTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDescriptionAsync_OnActiveInstancesCreatedByUser()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var description = "New description";

        await TestRecurringTaskSetterAsync<string>(tasks, groupId, userId, description, t => t.Description!.Value,
            _taskService.SetRecurringTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetTaskDescriptionAsync_OnTaskFromUserDirection()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId);
        var description = "New description";

        await TestTaskSetterAsync<string>(task, userId, description, t => t.Description!.Value,
            _taskService.SetTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDescriptionAsync_OnTaskInstancesFromUserDirection()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, directionId, null, null, groupId);
        var description = "New description";

        await TestRecurringTaskSetterAsync<string>(tasks, groupId, userId, description, t => t.Description!.Value,
            _taskService.SetRecurringTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetTaskDescriptionAsync_ThrowsWhenTaskNotFound()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var description = "New description";

        await TestTaskSetterNotFoundAsync(task, userId, description, _taskService.SetTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDescriptionAsync_ThrowsWhenTaskNotFound()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var description = "New description";

        await TestRecurringTaskSetterTaskNotFoundAsync(tasks, groupId, userId, description,
            _taskService.SetRecurringTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDescriptionAsync_ThrowsWhenGroupNotFound()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var description = "New description";

        await TestRecurringTaskSetterGroupNotFoundAsync(tasks, userId, description,
            _taskService.SetRecurringTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetTaskDescriptionAsync_ThrowsWhenNoAccessToTaskWithNoDirection()
    {
        var userId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var task = TaskFixture.GetTask(createdBy);
        var description = "New description";

        await TestTaskSetterNoAccessAsync(task, userId, description, _taskService.SetTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDescriptionAsync_ThrowsWhenNoAccessToTaskWithNoDirection()
    {
        var userId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(createdBy, null, null, null, groupId);
        var description = "New description";

        await TestRecurringTaskSetterNoAccessAsync(tasks, groupId, userId, description,
            _taskService.SetRecurringTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetTaskDescriptionAsync_ThrowsWhenNoAccessToTaskWithDirection()
    {
        var userId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(createdBy, directionId);
        var description = "New description";

        await TestTaskSetterNoAccessAsync(task, userId, description, _taskService.SetTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDescriptionAsync_ThrowsWhenNoAccessToTaskWithDirection()
    {
        var userId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(createdBy, directionId, null, null, groupId);
        var description = "New description";

        await TestRecurringTaskSetterNoAccessAsync(tasks, groupId, userId, description,
            _taskService.SetRecurringTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetTaskDescriptionAsync_ThrowsWhenTaskIsCompleted()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetCompletedTask(userId);
        var description = "New description";

        await TestTaskSetterUpdateFailsAsync(task, userId, description, _taskService.SetTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDescriptionAsync_ThrowsWhenSomeInstancesAreCompleted()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var completedTasks = TaskFixture.GetCompletedDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var description = "New description";

        await TestRecurringTaskSetterUpdateFailsAsync(tasks, completedTasks, groupId, userId, description,
            _taskService.SetRecurringTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetTaskDescriptionAsync_ThrowsWhenTaskIsDeleted()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetDeletedTask(userId);
        var description = "New description";

        await TestTaskSetterUpdateFailsAsync(task, userId, description, _taskService.SetTaskDescriptionAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDescriptionAsync_ThrowsWhenSomeInstancesAreDeleted()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var deletedTasks = TaskFixture.GetDeletedDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var description = "New description";

        await TestRecurringTaskSetterUpdateFailsAsync(tasks, deletedTasks, groupId, userId, description,
            _taskService.SetRecurringTaskDescriptionAsync);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task SetTaskDescriptionAsync_ThrowsWhenInvalidDescription(string description)
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);

        await TestTaskSetterValidationFailsAsync(task, userId, description, _taskService.SetTaskDescriptionAsync,
            ValidationErrors.InvalidDescription);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task SetRecurringTaskDescriptionAsync_ThrowsWhenInvalidDescription(string description)
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, null, null, null, groupId);

        await TestRecurringTaskSetterValidationFailsAsync(tasks, groupId, userId, description,
            _taskService.SetRecurringTaskDescriptionAsync, ValidationErrors.InvalidDescription);
    }

    [Fact]
    public async Task SetTaskDetailsAsync_OnTaskCreatedByUser()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var details = "New details";

        await TestTaskSetterAsync(task, userId, details, t => t.Details,
            _taskService.SetTaskDetailsAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDetailsAsync_OnActiveInstancesCreatedByUser()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var details = "New details";

        await TestRecurringTaskSetterAsync(tasks, groupId, userId, details, t => t.Details,
            _taskService.SetRecurringTaskDetailsAsync);
    }

    [Fact]
    public async Task SetTaskDetailsAsync_OnTaskFromUserDirection()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId, directionId);
        var details = "New details";

        await TestTaskSetterAsync(task, userId, details, t => t.Details, _taskService.SetTaskDetailsAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDetailsAsync_OnTaskInstancesFromUserDirection()
    {
        var userId = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, directionId, null, null, groupId);
        var details = "New details";

        await TestRecurringTaskSetterAsync(tasks, groupId, userId, details, t => t.Details,
            _taskService.SetRecurringTaskDetailsAsync);
    }

    [Fact]
    public async Task SetTaskDetailsAsync_ThrowsWhenTaskNotFound()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetTask(userId);
        var details = "New description";

        await TestTaskSetterNotFoundAsync(task, userId, details, _taskService.SetTaskDetailsAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDetailsAsync_ThrowsWhenTaskNotFound()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var details = "New description";

        await TestRecurringTaskSetterTaskNotFoundAsync(tasks, groupId, userId, details,
            _taskService.SetRecurringTaskDetailsAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDetailsAsync_ThrowsWhenGroupNotFound()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var details = "New description";

        await TestRecurringTaskSetterGroupNotFoundAsync(tasks, userId, details,
            _taskService.SetRecurringTaskDetailsAsync);
    }

    [Fact]
    public async Task SetTaskDetailsAsync_ThrowsWhenNoAccessToTaskWithNoDirection()
    {
        var userId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var task = TaskFixture.GetTask(createdBy);
        var details = "New description";

        await TestTaskSetterNoAccessAsync(task, userId, details, _taskService.SetTaskDetailsAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDetailsAsync_ThrowsWhenNoAccessToTaskWithNoDirection()
    {
        var userId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(createdBy, null, null, null, groupId);
        var details = "New description";

        await TestRecurringTaskSetterNoAccessAsync(tasks, groupId, userId, details,
            _taskService.SetRecurringTaskDetailsAsync);
    }

    [Fact]
    public async Task SetTaskDetailsAsync_ThrowsWhenNoAccessToTaskWithDirection()
    {
        var userId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var task = TaskFixture.GetTask(createdBy, directionId);
        var details = "New description";

        await TestTaskSetterNoAccessAsync(task, userId, details, _taskService.SetTaskDetailsAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDetailsAsync_ThrowsWhenNoAccessToTaskWithDirection()
    {
        var userId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var directionId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(createdBy, directionId, null, null, groupId);
        var details = "New description";

        await TestRecurringTaskSetterNoAccessAsync(tasks, groupId, userId, details,
            _taskService.SetRecurringTaskDetailsAsync);
    }

    [Fact]
    public async Task SetTaskDetailsAsync_ThrowsWhenTaskIsCompleted()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetCompletedTask(userId);
        var details = "New description";

        await TestTaskSetterUpdateFailsAsync(task, userId, details, _taskService.SetTaskDetailsAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDetailsAsync_ThrowsWhenSomeInstancesAreCompleted()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var completedTasks = TaskFixture.GetCompletedDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var details = "New description";

        await TestRecurringTaskSetterUpdateFailsAsync(tasks, completedTasks, groupId, userId, details,
            _taskService.SetRecurringTaskDetailsAsync);
    }

    [Fact]
    public async Task SetTaskDetailsAsync_ThrowsWhenTaskIsDeleted()
    {
        var userId = Guid.NewGuid();
        var task = TaskFixture.GetDeletedTask(userId);
        var details = "New description";

        await TestTaskSetterUpdateFailsAsync(task, userId, details, _taskService.SetTaskDetailsAsync);
    }

    [Fact]
    public async Task SetRecurringTaskDetailsAsync_ThrowsWhenSomeInstancesAreDeleted()
    {
        var userId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var tasks = TaskFixture.GetDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var deletedTasks = TaskFixture.GetDeletedDailyRecurringTaskInstances(userId, null, null, null, groupId);
        var details = "New description";

        await TestRecurringTaskSetterUpdateFailsAsync(tasks, deletedTasks, groupId, userId, details,
            _taskService.SetRecurringTaskDetailsAsync);
    }

    private async Task TestTaskSetterUpdateFailsAsync<T>(Models.Task task, Guid userId, T value,
        Func<Guid, T, Guid, Task<Guid?>> setter)
    {
        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var exception = await Assert.ThrowsAsync<DomainException>(() => setter(task.Id, value, userId));

        Assert.Equivalent(TaskErrors.UpdateCompletedOrDeletedTask, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.GetByIdAsync(task.Id), Times.Once);
    }

    private async Task TestTaskSetterNoAccessAsync<T>(Models.Task task, Guid userId, T value,
        Func<Guid, T, Guid, Task<Guid?>> setter)
    {
        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        if (task.DirectionId != null)
        {
            _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(task.DirectionId.Value, userId))
                .ReturnsAsync(false);
        }

        var exception = await Assert.ThrowsAsync<DomainException>(() => setter(task.Id, value, userId));

        Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.GetByIdAsync(task.Id), Times.Once);

        if (task.DirectionId != null)
        {
            _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(task.DirectionId.Value, userId), Times.Once);
        }
    }

    private async Task TestRecurringTaskSetterNoAccessAsync<T>(List<Models.Task> tasks, Guid groupId,
        Guid userId, T value, Func<Guid, Guid, T, Guid, Task<IEnumerable<Guid>>> setter)
    {
        _mockTasksRepository.Setup(r => r.GetActiveTasksAsync(groupId)).ReturnsAsync(tasks);

        if (tasks.First().DirectionId != null)
        {
            _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(tasks.First().DirectionId!.Value, userId))
                .ReturnsAsync(false);
        }

        var exception =
            await Assert.ThrowsAsync<DomainException>(() => setter(tasks.First().Id, groupId, value, userId));

        Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.GetActiveTasksAsync(groupId), Times.Once);

        if (tasks.First().DirectionId != null)
        {
            _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(tasks.First().DirectionId!.Value, userId),
                Times.Once);
        }
    }

    private async Task TestTaskSetterNotFoundAsync<T>(Models.Task task, Guid userId, T value,
        Func<Guid, T, Guid, Task<Guid?>> setter)
    {
        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync((Models.Task?) null);

        var exception = await Assert.ThrowsAsync<DomainException>(() => setter(task.Id, value, userId));

        Assert.Equivalent(TaskErrors.NotFound, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.GetByIdAsync(task.Id), Times.Once);
    }

    private async Task TestRecurringTaskSetterTaskNotFoundAsync<T>(List<Models.Task> tasks, Guid groupId,
        Guid userId, T value, Func<Guid, Guid, T, Guid, Task<IEnumerable<Guid>>> setter)
    {
        _mockTasksRepository.Setup(r => r.GetActiveTasksAsync(groupId)).ReturnsAsync(tasks);

        var exception = await Assert.ThrowsAsync<DomainException>(() => setter(Guid.NewGuid(), groupId, value, userId));

        Assert.Equivalent(TaskErrors.NotFound, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.GetActiveTasksAsync(groupId), Times.Once);
    }

    private async Task TestRecurringTaskSetterGroupNotFoundAsync<T>(List<Models.Task> tasks, Guid userId, T value,
        Func<Guid, Guid, T, Guid, Task<IEnumerable<Guid>>> setter)
    {
        var unknownGroupId = Guid.NewGuid();
        _mockTasksRepository.Setup(r => r.GetActiveTasksAsync(unknownGroupId)).ReturnsAsync(Array.Empty<Models.Task>);

        var exception =
            await Assert.ThrowsAsync<DomainException>(() => setter(tasks.First().Id, unknownGroupId, value, userId));

        Assert.Equivalent(TaskErrors.NotFound, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.GetActiveTasksAsync(unknownGroupId), Times.Once);
    }

    private async Task TestRecurringTaskSetterUpdateFailsAsync<T>(List<Models.Task> tasks, List<Models.Task> notActive,
        Guid groupId, Guid userId, T value, Func<Guid, Guid, T, Guid, Task<IEnumerable<Guid>>> setter)
    {
        _mockTasksRepository.Setup(r => r.GetActiveTasksAsync(groupId)).ReturnsAsync(tasks);

        var exception =
            await Assert.ThrowsAsync<DomainException>(() => setter(notActive.First().Id, groupId, value, userId));

        Assert.Equivalent(TaskErrors.NotFound, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.GetActiveTasksAsync(groupId), Times.Once);
    }

    private async Task TestTaskSetterValidationFailsAsync<T>(Models.Task task, Guid userId, T value,
        Func<Guid, T, Guid, Task<Guid?>> setter, ErrorDetails expected)
    {
        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        var exception = await Assert.ThrowsAsync<DomainException>(() => setter(task.Id, value, userId));

        Assert.Equivalent(expected, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.GetByIdAsync(task.Id), Times.Never);
    }

    private async Task TestRecurringTaskSetterValidationFailsAsync<T>(List<Models.Task> tasks, Guid groupId,
        Guid userId, T value, Func<Guid, Guid, T, Guid, Task<IEnumerable<Guid>>> setter, ErrorDetails expected)
    {
        _mockTasksRepository.Setup(r => r.GetActiveTasksAsync(groupId)).ReturnsAsync(tasks);

        var exception =
            await Assert.ThrowsAsync<DomainException>(() => setter(tasks.First().Id, groupId, value, userId));

        Assert.Equivalent(expected, exception.Error);

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockTasksRepository.Verify(r => r.GetActiveTasksAsync(groupId), Times.Never);
    }

    private async Task TestTaskSetterAsync<T>(Models.Task task, Guid userId, T value, Func<Models.Task, T> getter,
        Func<Guid, T, Guid, Task<Guid?>> setter)
    {
        _mockTasksRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);

        if (task.DirectionId.HasValue)
        {
            _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(task.DirectionId.Value, userId))
                .ReturnsAsync(true);
        }

        var result = await setter(task.Id, value, task.CreatedById);

        Assert.NotNull(result);
        Assert.Equal(task.Id, result);
        Assert.Equal(value, getter(task));

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockTasksRepository.Verify(r => r.GetByIdAsync(task.Id), Times.Once);

        if (task.DirectionId.HasValue)
        {
            _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(task.DirectionId.Value, userId),
                Times.Once);
        }
    }

    private async Task TestRecurringTaskSetterAsync<T>(List<Models.Task> tasks, Guid groupId, Guid userId, T value,
        Func<Models.Task, T> getter, Func<Guid, Guid, T, Guid, Task<IEnumerable<Guid>>> setter)
    {
        _mockTasksRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockTasksRepository.Setup(r => r.GetActiveTasksAsync(groupId)).ReturnsAsync(tasks);

        var task = tasks.First();

        if (task.DirectionId.HasValue)
        {
            _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(task.DirectionId.Value, userId))
                .ReturnsAsync(true);
        }

        var result = await setter(task.Id, groupId, value, userId);

        Assert.NotNull(result);
        Assert.All(result, id => Assert.Contains(id, tasks.Select(t => t.Id)));
        Assert.All(tasks, t => Assert.Equal(value, getter(t)));

        _mockTasksRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockTasksRepository.Verify(r => r.GetActiveTasksAsync(groupId), Times.Once);

        if (task.DirectionId.HasValue)
        {
            _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(task.DirectionId.Value, task.CreatedById),
                Times.Once);
        }
    }

    private static bool IsVisibleToUser(Models.Task task, Guid userId)
    {
        return task.CreatedById == userId ||
               task.Direction is not null && task.Direction.Members!.Any(m => m.Id == userId);
    }
}