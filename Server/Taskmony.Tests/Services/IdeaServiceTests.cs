using Moq;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Ideas;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;
using Taskmony.Services;
using Taskmony.Services.Abstract;
using Taskmony.Tests.Fixtures;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Tests.Services;

public class IdeaServiceTests
{
    private readonly Mock<IIdeaRepository> _mockIdeaRepository;
    private readonly Mock<IDirectionRepository> _mockDirectionRepository;
    private readonly Mock<ICommentRepository> _mockCommentRepository;
    private readonly IdeaService _ideaService;

    public IdeaServiceTests()
    {
        _mockIdeaRepository = new Mock<IIdeaRepository>();
        _mockDirectionRepository = new Mock<IDirectionRepository>();
        _mockCommentRepository = new Mock<ICommentRepository>();

        _ideaService = new IdeaService(_mockIdeaRepository.Object, _mockDirectionRepository.Object,
            new Mock<INotificationService>().Object, new TimeConverter(), _mockCommentRepository.Object);
    }

    [Fact]
    public async Task GetIdeasAsync_ReturnsAllUserIdeasWhenFiltersAreEmpty()
    {
        var userId = Guid.NewGuid();
        var ideas = IdeaFixture.GetIdeas(userId).ToList();
        var userDirectionIds = ideas.Where(i => i.DirectionId != null).Select(i => i.DirectionId).Distinct().ToList();
        var authorizedDirectionIds = userDirectionIds.Append(null);

        _mockDirectionRepository.Setup(r => r.GetUserDirectionIdsAsync(userId))
            .ReturnsAsync(userDirectionIds.Cast<Guid>());
        _mockIdeaRepository
            .Setup(r => r.GetAsync(null, authorizedDirectionIds.ToArray(), null, null, null, null, userId))
            .ReturnsAsync(ideas);

        var result = await _ideaService.GetIdeasAsync(null, null, null, null, null, null, userId);

        Assert.Equal(ideas, result);

        _mockIdeaRepository.Verify(
            r => r.GetAsync(null, authorizedDirectionIds.ToArray(), null, null, null, null, userId),
            Times.Once);
    }

    [Fact]
    public async Task GetIdeaOrThrowAsync_ReturnsIdeaCreatedByUser()
    {
        var userId = Guid.NewGuid();
        var idea = IdeaFixture.GetIdea(userId);

        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);

        var result = await _ideaService.GetIdeaOrThrowAsync(idea.Id, userId);

        Assert.Equal(idea, result);
    }

    [Fact]
    public async Task GetIdeaOrThrowAsync_ReturnsIdeaFromUserDirection()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        var idea = IdeaFixture.GetIdea(userId, direction.Id);

        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId)).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var result = await _ideaService.GetIdeaOrThrowAsync(idea.Id, userId);

        Assert.Equal(idea, result);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
    }

    [Fact]
    public async Task GetIdeaOrThrowAsync_ReturnsIdeaFromDirectionCreatedByUser()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        var idea = IdeaFixture.GetIdea(userId, direction.Id);

        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId)).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var result = await _ideaService.GetIdeaOrThrowAsync(idea.Id, userId);

        Assert.Equal(idea, result);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
    }

    [Fact]
    public async Task GetIdeaOrThrowAsync_ThrowsWhenIdeaIsPrivateAndCreatedByOtherUser()
    {
        var userId = Guid.NewGuid();
        var idea = IdeaFixture.GetIdea(Guid.NewGuid());

        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);

        var exception =
            await Assert.ThrowsAsync<DomainException>(() => _ideaService.GetIdeaOrThrowAsync(idea.Id, userId));

        Assert.Equivalent(exception.Error, GeneralErrors.Forbidden);
    }

    [Fact]
    public async Task GetTaskOrThrowAsync_ThrowsWhenIdeaIsFromInaccessibleDirection()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(Guid.NewGuid());
        var idea = IdeaFixture.GetIdea(Guid.NewGuid(), direction.Id);

        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(false);

        var exception =
            await Assert.ThrowsAsync<DomainException>(() => _ideaService.GetIdeaOrThrowAsync(idea.Id, userId));

        Assert.Equivalent(exception.Error, GeneralErrors.Forbidden);

        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
    }

    [Fact]
    public async Task AddIdeaAsync()
    {
        var userId = Guid.NewGuid();

        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _ideaService.AddIdeaAsync(
            description: "description",
            details: "details",
            currentUserId: userId,
            generation: Generation.Hot,
            directionId: null);

        Assert.NotNull(result);
        Assert.NotNull(result.Description);
        Assert.Equal("description", result.Description.Value);
        Assert.NotNull(result.Details);
        Assert.Equal("details", result.Details.Value);
        Assert.Equal(Generation.Hot, result.Generation);
        Assert.Null(result.DirectionId);
        Assert.NotNull(result.CreatedAt);
        Assert.Equal(userId, result.CreatedById);
        Assert.Null(result.ReviewedAt);
        Assert.Null(result.DeletedAt);

        _mockIdeaRepository.Verify(r => r.AddAsync(It.IsAny<Idea>()), Times.Once);
    }

    [Fact]
    public async Task AddIdeaAsync_ThrowsWhenDirectionIsDeleted()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        direction.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow));

        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _ideaService.AddIdeaAsync(
            description: "description",
            details: "details",
            currentUserId: userId,
            generation: Generation.Hot,
            directionId: direction.Id));

        Assert.Equivalent(exception.Error, DirectionErrors.UpdateDeletedDirection);

        _mockIdeaRepository.Verify(r => r.AddAsync(It.IsAny<Idea>()), Times.Never);
    }

    [Fact]
    public async Task SetIdeaDescriptionAsync()
    {
        var userId = Guid.NewGuid();
        var idea = IdeaFixture.GetIdea(userId);

        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _ideaService.SetIdeaDescriptionAsync(idea.Id, "New description", userId);

        Assert.NotNull(result);
        Assert.NotNull(idea.Description);
        Assert.Equal("New description", idea.Description.Value);

        _mockIdeaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetIdeaDetailsAsync()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        var idea = IdeaFixture.GetIdea(userId, direction.Id);

        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);

        var result = await _ideaService.SetIdeaDetailsAsync(idea.Id, "New details", userId);

        Assert.NotNull(result);
        Assert.NotNull(idea.Details);
        Assert.Equal("New details", idea.Details.Value);

        _mockIdeaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
    }

    [Fact]
    public async Task SetIdeaGenerationAsync()
    {
        var userId = Guid.NewGuid();
        var idea = IdeaFixture.GetIdea(userId);
        idea.UpdateGeneration(Generation.Hot);

        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _ideaService.SetIdeaGenerationAsync(idea.Id, Generation.Later, userId);

        Assert.NotNull(result);
        Assert.Equal(Generation.Later, idea.Generation);

        _mockIdeaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetIdeaReviewedAtAsync()
    {
        var userId = Guid.NewGuid();
        var idea = IdeaFixture.GetIdea(userId);
        var reviewedAt = DateTime.UtcNow;

        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _ideaService.SetIdeaReviewedAtAsync(idea.Id, reviewedAt, userId);

        Assert.NotNull(result);
        Assert.NotNull(idea.ReviewedAt);
        Assert.Equal(reviewedAt, idea.ReviewedAt.Value);

        _mockIdeaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetIdeaDirectionAsync_WhenOldIsNull()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        var idea = IdeaFixture.GetIdea(userId);

        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);

        var result = await _ideaService.SetIdeaDirectionAsync(idea.Id, direction.Id, userId);

        Assert.NotNull(result);
        Assert.Equal(direction.Id, idea.DirectionId);

        _mockIdeaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
    }

    [Fact]
    public async Task SetIdeaDirectionAsync_WhenOldIsNotNull()
    {
        var userId = Guid.NewGuid();
        var idea = IdeaFixture.GetIdea(userId);
        var direction = DirectionFixture.GetUserDirection(userId);
        var oldDirection = DirectionFixture.GetUserDirection(userId);
        idea.UpdateDirectionId(oldDirection.Id);

        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(oldDirection.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id)).ReturnsAsync(direction);

        var result = await _ideaService.SetIdeaDirectionAsync(idea.Id, direction.Id, userId);

        Assert.NotNull(result);
        Assert.NotNull(idea.DirectionId);
        Assert.Equal(direction.Id, idea.DirectionId);

        _mockIdeaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(direction.Id, userId), Times.Once);
        _mockDirectionRepository.Verify(r => r.GetByIdAsync(direction.Id), Times.Once);
    }

    [Fact]
    public async Task SetIdeaDirectionAsync_WhenNewIsNull()
    {
        var userId = Guid.NewGuid();
        var idea = IdeaFixture.GetIdea(userId);
        var oldDirection = DirectionFixture.GetUserDirection(userId);
        idea.UpdateDirectionId(oldDirection.Id);

        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(oldDirection.Id, userId))
            .ReturnsAsync(true);

        var result = await _ideaService.SetIdeaDirectionAsync(idea.Id, null, userId);

        Assert.NotNull(result);
        Assert.Null(idea.DirectionId);

        _mockIdeaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockDirectionRepository.Verify(r => r.AnyMemberWithIdAsync(oldDirection.Id, userId), Times.Once);
    }

    [Fact]
    public async Task SetIdeaDirectionAsync_ThrowsWhenDirectionIsDeleted()
    {
        var userId = Guid.NewGuid();
        var direction = DirectionFixture.GetUserDirection(userId);
        var idea = IdeaFixture.GetIdea(userId);
        direction.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow));

        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
            .ReturnsAsync(true);
        _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
            .ReturnsAsync(direction);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _ideaService.SetIdeaDirectionAsync(idea.Id, direction.Id, userId));

        Assert.Equivalent(DirectionErrors.UpdateDeletedDirection, exception.Error);

        _mockIdeaRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task SetTaskDeletedAtAsync_CallsSoftDeleteComments()
    {
        var userId = Guid.NewGuid();
        var idea = IdeaFixture.GetIdea(userId);
        var deletedAt = DateTime.UtcNow;

        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);

        var result = await _ideaService.SetIdeaDeletedAtAsync(idea.Id, deletedAt, userId);

        Assert.NotNull(result);
        Assert.NotNull(idea.DeletedAt);
        Assert.Equal(deletedAt, idea.DeletedAt.Value);

        _mockIdeaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockCommentRepository.Verify(r => r.SoftDeleteIdeaCommentsAsync(new[] {idea.Id}), Times.Once);
    }

    [Fact]
    public async Task SetTaskDeletedAtAsync_CallsUndeleteCommentsWhenSetToNull()
    {
        var userId = Guid.NewGuid();
        var idea = IdeaFixture.GetIdea(userId);
        var deletedAt = DateTime.UtcNow;
        idea.UpdateDeletedAt(DeletedAt.From(deletedAt));

        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);

        var result = await _ideaService.SetIdeaDeletedAtAsync(idea.Id, null, userId);

        Assert.NotNull(result);
        Assert.Null(idea.DeletedAt);

        _mockIdeaRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockCommentRepository.Verify(r => r.UndeleteIdeaCommentsAsync(new[] {idea.Id}, deletedAt), Times.Once);
    }

    [Fact]
    public async Task SetTaskDeletedAtAsync_ThrowsWhenAlreadyDeleted()
    {
        var userId = Guid.NewGuid();
        var idea = IdeaFixture.GetIdea(userId);
        idea.UpdateDeletedAt(DeletedAt.From(DateTime.UtcNow));

        _mockIdeaRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
        _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _ideaService.SetIdeaDeletedAtAsync(idea.Id, DateTime.UtcNow, userId));

        Assert.Equivalent(IdeaErrors.AlreadyDeleted, exception.Error);

        _mockIdeaRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        _mockCommentRepository.Verify(r => r.SoftDeleteIdeaCommentsAsync(It.IsAny<IEnumerable<Guid>>()), Times.Never);
    }
}