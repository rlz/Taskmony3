// using Moq;
// using Taskmony.Errors;
// using Taskmony.Exceptions;
// using Taskmony.Models.Ideas;
// using Taskmony.Repositories.Abstract;
// using Taskmony.Services;
// using Taskmony.Services.Abstract;
// using Taskmony.Tests.Fixtures;
// using Task = System.Threading.Tasks.Task;
//
// namespace Taskmony.Tests.Services;
//
// public class IdeaServiceTests
// {
//     private readonly Mock<IIdeaRepository> _mockIdeaRepository;
//     private readonly Mock<IDirectionRepository> _mockDirectionRepository;
//     private readonly IdeaService _ideaService;
//
//     public IdeaServiceTests()
//     {
//         _mockIdeaRepository = new Mock<IIdeaRepository>();
//         _mockDirectionRepository = new Mock<IDirectionRepository>();
//
//         _ideaService = new IdeaService(_mockIdeaRepository.Object, _mockDirectionRepository.Object,
//             new Mock<INotificationService>().Object, new TimeConverter(), new Mock<ICommentRepository>().Object);
//     }
//
//     [Fact]
//     public async Task GetIdeasAsync_ReturnsAllUserIdeasWhenFiltersAreEmpty()
//     {
//         var userId = Guid.NewGuid();
//
//         _mockIdeaRepository
//             .Setup(r => r.GetAsync(It.IsAny<Guid[]>(), It.IsAny<Guid?[]>(), It.IsAny<int>(), It.IsAny<int>(),
//                 It.IsAny<Guid>())).ReturnsAsync(IdeaFixture.GetIdeas(userId).Where(i => IsVisibleToUser(i, userId)));
//
//         var result = await _ideaService.GetIdeasAsync(null, null, null, null, userId);
//
//         Assert.IsAssignableFrom<IEnumerable<Idea>>(result);
//         Assert.All(result, i => Assert.True(IsVisibleToUser(i, userId)));
//     }
//
//     [Fact]
//     public async Task GetIdeaOrThrowAsync_ReturnsIdeaCreatedByUser()
//     {
//         var userId = Guid.NewGuid();
//         var ideas = IdeaFixture.GetIdeas(userId);
//         var idea = ideas.First(i => i.CreatedById == userId && i.Direction is null);
//
//         _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
//
//         var result = await _ideaService.GetIdeaOrThrowAsync(idea.Id, userId);
//
//         Assert.Equal(idea, result);
//     }
//
//     [Fact]
//     public async Task GetIdeaOrThrowAsync_ReturnsIdeaFromUserDirection()
//     {
//         var userId = Guid.NewGuid();
//         var ideas = IdeaFixture.GetIdeas(userId);
//         var idea = ideas.First(i =>
//             i.CreatedById != userId && i.Direction is not null && i.Direction.Members!.Any(m => m.Id == userId));
//
//         _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(idea.DirectionId!.Value, userId)).ReturnsAsync(true);
//
//         var result = await _ideaService.GetIdeaOrThrowAsync(idea.Id, userId);
//
//         Assert.Equal(idea, result);
//     }
//
//     [Fact]
//     public async Task GetIdeaOrThrowAsync_ReturnsIdeaFromDirectionCreatedByUser()
//     {
//         var userId = Guid.NewGuid();
//         var ideas = IdeaFixture.GetIdeas(userId);
//         var idea = ideas.First(i =>
//             i.Direction is not null && i.Direction.CreatedById == userId &&
//             i.Direction.Members!.Any(m => m.Id == userId));
//
//         _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(idea.DirectionId!.Value, userId)).ReturnsAsync(true);
//
//         var result = await _ideaService.GetIdeaOrThrowAsync(idea.Id, userId);
//
//         Assert.Equal(idea, result);
//     }
//
//     [Fact]
//     public async Task GetIdeaOrThrowAsync_ThrowsWhenIdeaIsPrivateAndCreatedByOtherUser()
//     {
//         var userId = Guid.NewGuid();
//         var ideas = IdeaFixture.GetIdeas(userId);
//         var idea = ideas.First(i => i.CreatedById != userId && i.Direction is null);
//
//         _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
//
//         var exception = await Assert.ThrowsAsync<DomainException>(() => _ideaService.GetIdeaOrThrowAsync(idea.Id, userId));
//         Assert.Equivalent(exception.Error, GeneralErrors.Forbidden);
//     }
//
//     [Fact]
//     public async Task GetTaskOrThrowAsync_ThrowsWhenTaskIsFromInaccessibleDirection()
//     {
//         var userId = Guid.NewGuid();
//         var ideas = IdeaFixture.GetIdeas(userId);
//         var idea = ideas.First(t => t.Direction is not null && t.Direction.Members!.All(m => m.Id != userId));
//
//         _mockIdeaRepository.Setup(r => r.GetByIdAsync(idea.Id)).ReturnsAsync(idea);
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(idea.DirectionId!.Value, userId))
//             .ReturnsAsync(false);
//
//         var exception = await Assert.ThrowsAsync<DomainException>(() => _ideaService.GetIdeaOrThrowAsync(idea.Id, userId));
//         Assert.Equivalent(exception.Error, GeneralErrors.Forbidden);
//     }
//
//     private static bool IsVisibleToUser(Idea idea, Guid userId)
//     {
//         return idea.CreatedById == userId ||
//                idea.Direction is not null && idea.Direction.Members!.Any(m => m.Id == userId);
//     }
// }