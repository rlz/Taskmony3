// using Moq;
// using Taskmony.Errors;
// using Taskmony.Exceptions;
// using Taskmony.Models.Directions;
// using Taskmony.Repositories.Abstract;
// using Taskmony.Services;
// using Taskmony.Services.Abstract;
// using Taskmony.Tests.Fixtures;
// using Task = System.Threading.Tasks.Task;
//
// namespace Taskmony.Tests.Services;
//
// public class DirectionServiceTests
// {
//     private readonly Mock<IDirectionRepository> _mockDirectionRepository;
//     private readonly Mock<IUserService> _mockUserService;
//     private readonly Mock<ITaskService> _mockTaskService;
//     private readonly DirectionService _directionService;
//
//     public DirectionServiceTests()
//     {
//         _mockDirectionRepository = new Mock<IDirectionRepository>();
//         _mockUserService = new Mock<IUserService>();
//         _mockTaskService = new Mock<ITaskService>();
//
//         _directionService = new DirectionService(_mockDirectionRepository.Object, _mockUserService.Object,
//             new Mock<INotificationService>().Object, _mockTaskService.Object, new Mock<IIdeaService>().Object);
//     }
//
//     [Fact]
//     public async Task AddDirectionAsync_AddsCreatorAsMember()
//     {
//         var direction = DirectionFixture.GetDirectionWithoutMembers();
//
//         var result = await _directionService.AddDirectionAsync(direction);
//
//         _mockDirectionRepository.Verify(
//             r => r.AddMemberAsync(It.Is<Membership>(m =>
//                 m.DirectionId == direction.Id && m.UserId == direction.CreatedById)), Times.Once);
//         _mockDirectionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
//
//         Assert.Equal(direction, result);
//     }
//
//     [Fact]
//     public async Task AddMemberAsync_ThrowsWhenUserIsAlreadyMember()
//     {
//         var userId = Guid.NewGuid();
//         var otherUserId = Guid.NewGuid();
//         var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId, otherUserId);
//
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, It.IsAny<Guid>()))
//             .ReturnsAsync(true);
//         _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
//             .ReturnsAsync(direction);
//
//         var exception = await Assert.ThrowsAsync<DomainException>(() =>
//             _directionService.AddMemberAsync(direction.Id, otherUserId, userId));
//         Assert.Equivalent(DirectionErrors.UserIsAlreadyMember, exception.Error);
//     }
//
//     [Fact]
//     public async Task AddMemberAsync_ReturnsDirectionIdWhenMemberIsAdded()
//     {
//         var userId = Guid.NewGuid();
//         var otherUserId = Guid.NewGuid();
//         var direction = DirectionFixture.GetPrivateUserDirection(userId);
//
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId))
//             .ReturnsAsync(false);
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
//             .ReturnsAsync(true);
//         _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
//             .ReturnsAsync(direction);
//         _mockUserService.Setup(s => s.GetUserOrThrowAsync(otherUserId))
//             .ReturnsAsync(UserFixture.GetUserWithId(otherUserId));
//         _mockDirectionRepository.Setup(r => r.SaveChangesAsync())
//             .ReturnsAsync(true);
//
//         var result = await _directionService.AddMemberAsync(direction.Id, otherUserId, userId);
//
//         _mockDirectionRepository.Verify(
//             r => r.AddMemberAsync(It.Is<Membership>(m =>
//                 m.DirectionId == direction.Id && m.UserId == otherUserId)), Times.Once);
//
//         Assert.Equal(direction.Id, result);
//     }
//
//     [Fact]
//     public async Task AddMemberAsync_ThrowsWhenCurrentUserIsNotMemberOfDirection()
//     {
//         var userId = Guid.NewGuid();
//         var otherUserId = Guid.NewGuid();
//         var direction = DirectionFixture.GetPrivateUserDirection(userId);
//
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId))
//             .ReturnsAsync(false);
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
//             .ReturnsAsync(false);
//         _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
//             .ReturnsAsync(direction);
//         _mockUserService.Setup(s => s.GetUserOrThrowAsync(otherUserId))
//             .ReturnsAsync(UserFixture.GetUserWithId(otherUserId));
//
//         var exception = await Assert.ThrowsAsync<DomainException>(() =>
//             _directionService.AddMemberAsync(direction.Id, otherUserId, userId));
//         Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);
//     }
//
//     [Fact]
//     public async Task RemoveMemberAsync_ReturnsDirectionIdWhenMemberIsRemoved()
//     {
//         var userId = Guid.NewGuid();
//         var otherUserId = Guid.NewGuid();
//         var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId, otherUserId);
//
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId))
//             .ReturnsAsync(true);
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
//             .ReturnsAsync(true);
//         _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
//             .ReturnsAsync(direction);
//         _mockUserService.Setup(s => s.GetUserOrThrowAsync(otherUserId))
//             .ReturnsAsync(UserFixture.GetUserWithId(otherUserId));
//         _mockDirectionRepository.Setup(r => r.SaveChangesAsync())
//             .ReturnsAsync(true);
//
//         var result = await _directionService.RemoveMemberAsync(direction.Id, otherUserId, userId);
//
//         _mockDirectionRepository.Verify(
//             r => r.RemoveMember(It.Is<Membership>(m =>
//                 m.DirectionId == direction.Id && m.UserId == otherUserId)), Times.Once);
//
//         Assert.Equal(direction.Id, result);
//     }
//
//     [Fact]
//     public async Task RemoveMemberAsync_ThrowsWhenCurrentUserIsNotMemberOfDirection()
//     {
//         var userId = Guid.NewGuid();
//         var otherUserId = Guid.NewGuid();
//         var direction = DirectionFixture.GetPrivateUserDirection(userId);
//
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId))
//             .ReturnsAsync(false);
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
//             .ReturnsAsync(false);
//         _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
//             .ReturnsAsync(direction);
//         _mockUserService.Setup(s => s.GetUserOrThrowAsync(otherUserId))
//             .ReturnsAsync(UserFixture.GetUserWithId(otherUserId));
//
//         var exception = await Assert.ThrowsAsync<DomainException>(() =>
//             _directionService.RemoveMemberAsync(direction.Id, otherUserId, userId));
//         Assert.Equivalent(GeneralErrors.Forbidden, exception.Error);
//     }
//
//     [Fact]
//     public async Task RemoveMemberAsync_CallsRemoveAssignee()
//     {
//         var userId = Guid.NewGuid();
//         var otherUserId = Guid.NewGuid();
//         var direction = DirectionFixture.GetPublicDirectionCreatedByUser(userId, otherUserId);
//
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, otherUserId))
//             .ReturnsAsync(true);
//         _mockDirectionRepository.Setup(r => r.AnyMemberWithIdAsync(direction.Id, userId))
//             .ReturnsAsync(true);
//         _mockDirectionRepository.Setup(r => r.GetByIdAsync(direction.Id))
//             .ReturnsAsync(direction);
//         _mockUserService.Setup(s => s.GetUserOrThrowAsync(otherUserId))
//             .ReturnsAsync(UserFixture.GetUserWithId(otherUserId));
//         _mockDirectionRepository.Setup(r => r.SaveChangesAsync())
//             .ReturnsAsync(true);
//         
//         var id = await _directionService.RemoveMemberAsync(direction.Id, otherUserId, userId);
//
//         Assert.NotNull(id);
//         Assert.Equal(direction.Id, id);
//         _mockTaskService.Verify(s => s.RemoveAssigneeFromDirectionTasksAsync(otherUserId, direction.Id), Times.Once);
//     }
// }