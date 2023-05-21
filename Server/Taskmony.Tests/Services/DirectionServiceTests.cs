using Moq;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Directions;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;
using Taskmony.Services;
using Taskmony.Services.Abstract;
using Taskmony.Tests.Fixtures;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Tests.Services;

public class DirectionServiceTests
{
    private readonly Mock<IDirectionRepository> _mockDirectionRepository;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ITaskService> _mockTaskService;
    private readonly DirectionService _directionService;
    private readonly Mock<IIdeaService> _mockIdeaService;

    public DirectionServiceTests()
    {
        _mockDirectionRepository = new Mock<IDirectionRepository>();
        _mockUserService = new Mock<IUserService>();
        _mockTaskService = new Mock<ITaskService>();
        _mockIdeaService = new Mock<IIdeaService>();

        _directionService = new DirectionService(_mockDirectionRepository.Object, _mockUserService.Object,
            new Mock<INotificationService>().Object, _mockTaskService.Object, _mockIdeaService.Object);
    }

    [Fact]
    public async Task AddDirectionAsync_AddsCreatorAsMember()
    {
        var userId = Guid.NewGuid();

        var result = await _directionService.AddDirectionAsync("name", null, userId);

        Assert.Equal(userId, result.CreatedById);
        Assert.NotNull(result.Name);
        Assert.Equal("name", result.Name.Value);
        Assert.Equal(userId, result.CreatedById);
        Assert.NotNull(result.Details);
        Assert.Null(result.Details.Value);

        _mockDirectionRepository.Verify(
            r => r.AddMemberAsync(It.Is<Membership>(m =>
                m.DirectionId == result.Id && m.UserId == result.CreatedById)), Times.Once);
        _mockDirectionRepository.Verify(r => r.AddAsync(result), Times.Once);
        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddMemberAsync_ThrowsWhenUserIsAlreadyMember()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockUserService.Setup(s => s.GetUserOrThrowAsync(otherUserId))
            .ReturnsAsync(UserFixture.GetUser(otherUserId));

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _directionService.AddMemberAsync(direction.Id, otherUserId, userId));
        Assert.Equivalent(DirectionErrors.UserIsAlreadyMember, exception.Error);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId), Times.Once);
    }

    [Fact]
    public async Task AddMemberAsync_ReturnsDirectionIdWhenMemberIsAdded()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var direction = DirectionFixture.GetPrivateUserDirection(userId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId))
            .ReturnsAsync(false);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockUserService.Setup(s => s.GetUserOrThrowAsync(otherUserId))
            .ReturnsAsync(UserFixture.GetUser(otherUserId));
        _mockDirectionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);

        var result = await _directionService.AddMemberAsync(direction.Id, otherUserId, userId);

        _mockDirectionRepository.Verify(
            r => r.AddMemberAsync(It.Is<Membership>(m =>
                m.DirectionId == direction.Id && m.UserId == otherUserId)), Times.Once);

        Assert.Equal(direction.Id, result);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddMemberAsync_ThrowsWhenCurrentUserIsNotMemberOfDirection()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var direction = DirectionFixture.GetPrivateUserDirection(userId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId))
            .ReturnsAsync(false);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(false);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockUserService.Setup(s => s.GetUserOrThrowAsync(otherUserId))
            .ReturnsAsync(UserFixture.GetUser(otherUserId));

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _directionService.AddMemberAsync(direction.Id, otherUserId, userId));
        Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task RemoveMemberAsync_ReturnsDirectionIdWhenMemberIsRemoved()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockUserService.Setup(s => s.GetUserOrThrowAsync(otherUserId))
            .ReturnsAsync(UserFixture.GetUser(otherUserId));
        _mockDirectionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberInDirectionAsync(direction.Id))
            .ReturnsAsync(true);

        var result = await _directionService.RemoveMemberAsync(direction.Id, otherUserId, userId);

        _mockDirectionRepository.Verify(
            r => r.RemoveMember(It.Is<Membership>(m =>
                m.DirectionId == direction.Id && m.UserId == otherUserId)), Times.Once);

        Assert.Equal(direction.Id, result);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoveMemberAsync_ThrowsWhenCurrentUserIsNotMemberOfDirection()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var direction = DirectionFixture.GetPrivateUserDirection(userId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId))
            .ReturnsAsync(false);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(false);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockUserService.Setup(s => s.GetUserOrThrowAsync(otherUserId))
            .ReturnsAsync(UserFixture.GetUser(otherUserId));

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _directionService.RemoveMemberAsync(direction.Id, otherUserId, userId));
        Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task RemoveMemberAsync_CallsRemoveAssignee()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockUserService.Setup(s => s.GetUserOrThrowAsync(otherUserId))
            .ReturnsAsync(UserFixture.GetUser(otherUserId));
        _mockDirectionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);

        var id = await _directionService.RemoveMemberAsync(direction.Id, otherUserId, userId);

        Assert.NotNull(id);
        Assert.Equal(direction.Id, id);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockTaskService.Verify(s => s.RemoveAssigneeFromDirectionTasksAsync(otherUserId, direction.Id), Times.Once);
    }

    [Fact]
    public async Task RemoveMemberAsync_DeletesDirectionWhenLastMemberIsRemoved()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId);

        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockDirectionRepository.Setup(r => r.AnyMemberInDirectionAsync(direction.Id))
            .ReturnsAsync(false);
        _mockUserService.Setup(s => s.GetUserOrThrowAsync(userId))
            .ReturnsAsync(UserFixture.GetUser(userId));
        _mockDirectionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);

        var id = await _directionService.RemoveMemberAsync(direction.Id, userId, userId);

        Assert.NotNull(id);
        Assert.Equal(direction.Id, id);

        _mockDirectionRepository.Verify(r => r.Delete(direction), Times.Once);
        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Exactly(2));
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
    }

    [Fact]
    public async Task SetDirectionNameAsync_ReturnsDirectionIdWhenNameIsSet()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId);
        var newName = "new name";

        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);

        var result = await _directionService.SetDirectionNameAsync(direction.Id, newName, userId);

        Assert.Equal(direction.Id, result);
        Assert.NotNull(direction.Name);
        Assert.Equal(direction.Name.Value, newName);

        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
    }

    [Fact]
    public async Task SetDirectionNameAsync_ThrowsWhenCurrentUserIsNotMemberOfDirection()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(Guid.NewGuid());
        var newName = "new name";

        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _directionService.SetDirectionNameAsync(direction.Id, newName, userId));
        Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);

        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
    }

    [Fact]
    public async Task SetDirectionNameAsync_ThrowsWhenDirectionDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId);

        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(null as Direction);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _directionService.SetDirectionNameAsync(direction.Id, "newName", userId));
        Assert.Equivalent(DirectionErrors.NotFound, exception.Error);

        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Never);
    }

    [Fact]
    public async Task SetDirectionDetailsAsync_ReturnsDirectionIdWhenDetailsAreSet()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId);
        var newDetails = "new details";

        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);

        var result = await _directionService.SetDirectionDetailsAsync(direction.Id, newDetails, userId);

        Assert.Equal(direction.Id, result);
        Assert.NotNull(direction.Details);
        Assert.Equal(direction.Details.Value, newDetails);

        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
    }

    [Fact]
    public async Task SetDirectionDetailsAsync_ThrowsWhenCurrentUserIsNotMemberOfDirection()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(Guid.NewGuid());
        var newDetails = "new details";

        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _directionService.SetDirectionDetailsAsync(direction.Id, newDetails, userId));
        Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);

        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
    }

    [Fact]
    public async Task SetDirectionDetailsAsync_ThrowsWhenDirectionDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId);

        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(null as Direction);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _directionService.SetDirectionDetailsAsync(direction.Id, "newDetails", userId));
        Assert.Equivalent(DirectionErrors.NotFound, exception.Error);

        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Never);
    }

    [Fact]
    public async Task SetDirectionDeletedAtAsync_CallsSoftDeleteTasksAndIdeasWhenDeletedAtIsNotNull()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId);

        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);

        var result = await _directionService.SetDirectionDeletedAtAsync(direction.Id, DateTime.UtcNow, userId);

        Assert.NotNull(result);
        Assert.Equal(direction.Id, result);
        Assert.NotNull(direction.DeletedAt);

        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockTaskService.Verify(r => r.SoftDeleteDirectionTasksAsync(direction.Id), Times.Once);
        _mockIdeaService.Verify(r => r.SoftDeleteDirectionIdeasAsync(direction.Id), Times.Once);
    }

    [Fact]
    public async Task SetDirectionDeletedAtAsync_CallsUndeleteTasksAndIdeasWhenDeletedAtIsNull()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId);
        var deletedAt = DateTime.UtcNow;
        direction.UpdateDeletedAt(DeletedAt.From(deletedAt));

        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(true);

        var result = await _directionService.SetDirectionDeletedAtAsync(direction.Id, null, userId);

        Assert.NotNull(result);
        Assert.Equal(direction.Id, result);
        Assert.Null(direction.DeletedAt);

        _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockTaskService.Verify(r => r.UndeleteDirectionTasksAsync(direction.Id, deletedAt), Times.Once);
        _mockIdeaService.Verify(r => r.UndeleteDirectionIdeasAsync(direction.Id, deletedAt), Times.Once);
    }
}