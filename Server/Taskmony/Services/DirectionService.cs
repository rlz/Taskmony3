using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Directions;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;

namespace Taskmony.Services;

public class DirectionService : IDirectionService
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly ITaskService _taskService;
    private readonly IIdeaService _ideaService;

    public DirectionService(IDirectionRepository directionRepository, IUserService userService,
        INotificationService notificationService, ITaskService taskService, IIdeaService ideaService)
    {
        _ideaService = ideaService;
        _directionRepository = directionRepository;
        _userService = userService;
        _notificationService = notificationService;
        _taskService = taskService;
    }

    public async Task<ILookup<Guid, Guid>> GetMemberIdsAsync(Guid[] directionIds, int? offset, int? limit)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        return await _directionRepository.GetMemberIdsAsync(directionIds, offsetValue, limitValue);
    }

    public async Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, bool? deleted, DateTime? lastDeletedAt,
        int? offset, int? limit, Guid currentUserId)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        return await _directionRepository.GetAsync(
            id: id,
            deleted: deleted ?? false,
            lastDeletedAt: lastDeletedAt,
            offset: offsetValue,
            limit: limitValue,
            userId: currentUserId);
    }

    public async Task<IEnumerable<Direction>> GetDirectionsByIdsAsync(Guid[] ids)
    {
        return await _directionRepository.GetByIdsAsync(ids);
    }

    public async Task<Direction> AddDirectionAsync(string name, string? details, Guid currentUserId)
    {
        var direction = new Direction(currentUserId, DirectionName.From(name), details);

        await _directionRepository.AddAsync(direction);

        // Add creator as a member
        await _directionRepository.AddMemberAsync(new Membership(direction.Id, direction.CreatedById));

        await _directionRepository.SaveChangesAsync();

        return direction;
    }

    public async Task<Guid?> AddMemberAsync(Guid directionId, Guid memberId, Guid currentUserId)
    {
        var direction = await GetDirectionOrThrowAsync(directionId, currentUserId);

        direction.ValidateDirectionToUpdate();

        var user = await _userService.GetUserOrThrowAsync(memberId);

        if (await _directionRepository.AnyMemberWithIdAsync(directionId, memberId))
        {
            throw new DomainException(DirectionErrors.UserIsAlreadyMember);
        }

        var membership = new Membership(direction.Id, user.Id);

        await _directionRepository.AddMemberAsync(membership);

        if (!await _directionRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionMemberAddedAsync(direction.Id, user.Id, membership.CreatedAt,
            currentUserId);

        return direction.Id;
    }

    public async Task<Guid?> RemoveMemberAsync(Guid directionId, Guid memberId, Guid currentUserId)
    {
        var direction = await GetDirectionOrThrowAsync(directionId, currentUserId);

        direction.ValidateDirectionToUpdate();

        var user = await _userService.GetUserOrThrowAsync(memberId);

        _directionRepository.RemoveMember(new Membership(direction.Id, user.Id));

        if (!await _directionRepository.SaveChangesAsync())
        {
            return null;
        }

        await _taskService.RemoveAssigneeFromDirectionTasksAsync(user.Id, direction.Id);

        await _notificationService.NotifyDirectionMemberRemovedAsync(direction.Id, user.Id, null, currentUserId);

        await DeleteDirectionIfNoMembersAsync(direction);

        return direction.Id;
    }

    public async Task<Guid?> SetDirectionDeletedAtAsync(Guid id, DateTime? deletedAtUtc, Guid currentUserId)
    {
        var deletedAt = deletedAtUtc != null ? DeletedAt.From(deletedAtUtc.Value) : null;

        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        var oldDeletedAt = direction.DeletedAt;
        direction.UpdateDeletedAt(deletedAt);

        if (!await _directionRepository.SaveChangesAsync())
        {
            return null;
        }

        if (direction.DeletedAt != null)
        {
            await _taskService.SoftDeleteDirectionTasksAsync(direction.Id);
            await _ideaService.SoftDeleteDirectionIdeasAsync(direction.Id);
        }
        else if (oldDeletedAt != null)
        {
            await _taskService.UndeleteDirectionTasksAsync(direction.Id, oldDeletedAt.Value);
            await _ideaService.UndeleteDirectionIdeasAsync(direction.Id, oldDeletedAt.Value);
        }

        return direction.Id;
    }

    public async Task<Guid?> SetDirectionDetailsAsync(Guid id, string? details, Guid currentUserId)
    {
        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        var oldValue = direction.Details;
        direction.UpdateDetails(details);

        if (!await _directionRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionUpdatedAsync(direction.Id, nameof(Direction.Details), oldValue,
            details, currentUserId);

        return direction.Id;
    }

    public async Task<Guid?> SetDirectionNameAsync(Guid id, string name, Guid currentUserId)
    {
        var newName = DirectionName.From(name);

        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        var oldValue = direction.Name!.Value;
        direction.UpdateName(newName);

        if (!await _directionRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionUpdatedAsync(direction.Id, nameof(Direction.Name), oldValue,
            newName.Value, currentUserId);

        return direction.Id;
    }

    private async Task<bool> DeleteDirectionIfNoMembersAsync(Direction direction)
    {
        if (await _directionRepository.AnyMemberInDirectionAsync(direction.Id))
        {
            return false;
        }

        _directionRepository.Delete(direction);

        await _ideaService.SoftDeleteDirectionIdeasAsync(direction.Id);
        await _taskService.SoftDeleteDirectionTasksAsync(direction.Id);

        return await _directionRepository.SaveChangesAsync();
    }

    private async Task<Direction> GetDirectionOrThrowAsync(Guid id, Guid currentUserId)
    {
        var direction = await _directionRepository.GetByIdAsync(id);

        if (direction == null)
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        if (!await _directionRepository.AnyMemberWithIdAsync(id, currentUserId))
        {
            throw new DomainException(GeneralErrors.Forbidden);
        }

        return direction;
    }
}