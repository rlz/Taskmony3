using Moq;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Repositories.Abstract;
using Taskmony.Services;
using Taskmony.Services.Abstract;
using Taskmony.Tests.Fixtures;

namespace Taskmony.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTasksRepository;
    private readonly Mock<IDirectionRepository> _mockDirectionRepository;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockTasksRepository = new Mock<ITaskRepository>();
        _mockDirectionRepository = new Mock<IDirectionRepository>();

        _taskService = new TaskService(_mockTasksRepository.Object, _mockDirectionRepository.Object,
            new Mock<INotificationService>().Object, new TimeConverter());
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

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.GetTaskOrThrowAsync(task.Id, userId));
        Assert.Equivalent(exception.Error, GeneralErrors.Forbidden);
    }

    [Fact]
    public async Task GetTaskOrThrowAsync_ThrowsWhenTaskIsFromInaccessibleDirection()
    {
        var userId = Guid.NewGuid();
        var tasks = TaskFixture.GetTasks(userId);
        var task = tasks.First(t => t.Direction is not null && t.Direction.Members!.All(m => m.Id != userId));

        _mockTasksRepository.Setup(r => r.GetByIdAsync(task.Id)).ReturnsAsync(task);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(task.DirectionId!.Value, userId))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _taskService.GetTaskOrThrowAsync(task.Id, userId));
        Assert.Equivalent(exception.Error, GeneralErrors.Forbidden);
    }

    private static bool IsVisibleToUser(Models.Task task, Guid userId)
    {
        return task.CreatedById == userId ||
               task.Direction is not null && task.Direction.Members!.Any(m => m.Id == userId);
    }
}