using Moq;
using Taskmony.Repositories.Abstract;
using Taskmony.Services;
using Taskmony.Services.Abstract;
using Taskmony.Tests.Fixtures;

namespace Taskmony.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTasksRepository;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<IDirectionRepository> _mockDirectionRepository;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockTasksRepository = new Mock<ITaskRepository>();
        _mockNotificationService = new Mock<INotificationService>();
        _mockDirectionRepository = new Mock<IDirectionRepository>();

        _taskService = new TaskService(_mockTasksRepository.Object, _mockDirectionRepository.Object,
            _mockNotificationService.Object, new TimeConverter());
    }

    [Fact]
    public async Task GetTasksAsync_ReturnsAllUserTasksWhenFiltersAreEmpty()
    {
        var userId = Guid.NewGuid();

        _mockTasksRepository
            .Setup(r => r.GetAsync(It.IsAny<Guid[]>(), It.IsAny<Guid?[]>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<Guid>())).ReturnsAsync(TasksFixture.GetTasks(userId));

        var result = await _taskService.GetTasksAsync(null, null, null, null, Guid.NewGuid());

        Assert.IsType<List<Models.Task>>(result);
        Assert.All(result, t => Assert.True(IsVisibleToUser(t, userId)));
    }

    private bool IsVisibleToUser(Models.Task task, Guid userId)
    {
        return task.CreatedById == userId || task.Direction is not null && task.Direction.Members!.Any(m => m.Id == userId);
    }
}