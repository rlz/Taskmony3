using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;
using Taskmony.ValueObjects;

namespace Taskmony.Services;

public class DirectionService : IDirectionService
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;

    public DirectionService(IDirectionRepository directionRepository, IUserService userService,
        INotificationService notificationService)
    {
        _directionRepository = directionRepository;
        _userService = userService;
        _notificationService = notificationService;
    }

    public async Task<bool> AnyMemberWithIdAsync(Guid directionId, Guid memberId)
    {
        return await _directionRepository.AnyMemberWithIdAsync(directionId, memberId);
    }

    public async Task<ILookup<Guid, Guid>> GetMemberIdsAsync(Guid[] directionIds, int? offset, int? limit)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        return await _directionRepository.GetMemberIdsAsync(directionIds, offsetValue, limitValue);
    }

    public async Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, int? offset, int? limit,
        Guid currentUserId)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        return await _directionRepository.GetDirectionsAsync(id, offsetValue, limitValue, currentUserId);
    }

    public async Task<IEnumerable<Direction>> GetDirectionsByIdsAsync(Guid[] ids)
    {
        return await _directionRepository.GetDirectionByIdsAsync(ids);
    }

    public async Task<IEnumerable<Guid>> GetUserDirectionIds(Guid userId)
    {
        return await _directionRepository.GetUserDirectionIds(userId);
    }

    public async Task<Direction> AddDirection(Direction direction)
    {
        await _directionRepository.AddDirectionAsync(direction);

        await _directionRepository.AddMemberAsync(new Membership
        {
            DirectionId = direction.Id,
            UserId = direction.CreatedById
        });

        await _directionRepository.SaveChangesAsync();

        return direction;
    }

    public async Task<Guid?> AddMember(Guid directionId, Guid memberId, Guid currentUserId)
    {
        var direction = await GetDirectionOrThrowAsync(directionId, currentUserId);

        ValidateDirectionToUpdate(direction);

        var user = await _userService.GetUserOrThrowAsync(memberId);

        if (await _directionRepository.AnyMemberWithIdAsync(directionId, memberId))
        {
            throw new DomainException(DirectionErrors.UserIsAlreadyMember);
        }

        var membership = new Membership
        {
            DirectionId = direction.Id,
            UserId = user.Id
        };

        await _directionRepository.AddMemberAsync(membership);

        if (!await _directionRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyItemAddedAsync(NotifiableType.Direction, direction.Id,
            user.ActionItemType, user.Id, currentUserId, membership.CreatedAt);

        return direction.Id;
    }

    public async Task<Guid?> RemoveMember(Guid directionId, Guid memberId, Guid currentUserId)
    {
        var direction = await GetDirectionOrThrowAsync(directionId, currentUserId);

        ValidateDirectionToUpdate(direction);

        var user = await _userService.GetUserOrThrowAsync(memberId);

        _directionRepository.RemoveMember(new Membership
        {
            DirectionId = direction.Id,
            UserId = user.Id
        });

        if (!await _directionRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyItemRemovedAsync(NotifiableType.Direction, direction.Id,
            user.ActionItemType, user.Id, currentUserId, null);

        return direction.Id;
    }

    public async Task<Guid?> SetDirectionDeletedAt(Guid id, DateTime? deletedAtUtc, Guid currentUserId)
    {
        var deletedAt = deletedAtUtc is not null ? DeletedAt.From(deletedAtUtc.Value) : null;

        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        if (deletedAtUtc is not null && direction.DeletedAt is not null)
        {
            throw new DomainException(DirectionErrors.AlreadyDeleted);
        }

        direction.DeletedAt = deletedAt;

        return await _directionRepository.SaveChangesAsync() ? direction.Id : null;
    }

    public async Task<Guid?> SetDirectionDetails(Guid id, string? details, Guid currentUserId)
    {
        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        ValidateDirectionToUpdate(direction);

        var oldValue = direction.Details;
        direction.Details = details;

        return await SaveChangesAndNotifyAsync(direction, nameof(Direction.Details), oldValue, details, currentUserId);
    }

    public async Task<Guid?> SetDirectionName(Guid id, string name, Guid currentUserId)
    {
        var newName = DirectionName.From(name);

        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        ValidateDirectionToUpdate(direction);

        var oldValue = direction.Name!.Value;
        direction.Name = newName;

        return await SaveChangesAndNotifyAsync(direction, nameof(Direction.Name), oldValue, name, currentUserId);
    }

    private async Task<Guid?> SaveChangesAndNotifyAsync(Direction direction, string field, string? oldValue,
        string? newValue, Guid currentUserId)
    {
        if (!await _directionRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyItemUpdatedAsync(NotifiableType.Direction, direction.Id, currentUserId, field,
            oldValue, newValue);

        return direction.Id;
    }

    private async Task<Direction> GetDirectionOrThrowAsync(Guid id, Guid currentUserId)
    {
        var direction = await _directionRepository.GetDirectionByIdAsync(id);

        if (direction is null || !await _directionRepository.AnyMemberWithIdAsync(id, currentUserId))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        return direction;
    }

    private void ValidateDirectionToUpdate(Direction direction)
    {
        if (direction.DeletedAt is not null)
        {
            throw new DomainException(IdeaErrors.UpdateDeletedIdea);
        }
    }
}