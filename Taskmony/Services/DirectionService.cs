using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models;
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

        return await _directionRepository.GetAsync(id, offsetValue, limitValue, currentUserId);
    }

    public async Task<IEnumerable<Direction>> GetDirectionsByIdsAsync(Guid[] ids)
    {
        return await _directionRepository.GetByIdsAsync(ids);
    }

    public async Task<IEnumerable<Guid>> GetUserDirectionIdsAsync(Guid userId)
    {
        return await _directionRepository.GetUserDirectionIds(userId);
    }

    public async Task<Direction> AddDirectionAsync(Direction direction)
    {
        await _directionRepository.AddAsync(direction);

        await _directionRepository.AddMemberAsync(new Membership
        {
            DirectionId = direction.Id,
            UserId = direction.CreatedById
        });

        await _directionRepository.SaveChangesAsync();

        return direction;
    }

    public async Task<Guid?> AddMemberAsync(Guid directionId, Guid memberId, Guid currentUserId)
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

        await _notificationService.NotifyDirectionMemberAddedAsync(direction.Id, user.Id, membership.CreatedAt,
            currentUserId);

        return direction.Id;
    }

    public async Task<Guid?> RemoveMemberAsync(Guid directionId, Guid memberId, Guid currentUserId)
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

        await _notificationService.NotifyDirectionMemberRemovedAsync(direction.Id, user.Id, null, currentUserId);

        await DeleteDirectionIfNoMembersAsync(direction);

        return direction.Id;
    }

    public async Task<Guid?> SetDirectionDeletedAtAsync(Guid id, DateTime? deletedAtUtc, Guid currentUserId)
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

        if (!await _directionRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionUpdatedAsync(direction.Id, nameof(Direction.Details), oldValue,
            details, currentUserId);

        return direction.Id;
    }

    public async Task<Guid?> SetDirectionName(Guid id, string name, Guid currentUserId)
    {
        var newName = DirectionName.From(name);

        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        ValidateDirectionToUpdate(direction);

        var oldValue = direction.Name!.Value;
        direction.Name = newName;

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

        return await _directionRepository.SaveChangesAsync();
    }

    private async Task<Direction> GetDirectionOrThrowAsync(Guid id, Guid currentUserId)
    {
        var direction = await _directionRepository.GetByIdAsync(id);

        if (direction is null)
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        if (!await _directionRepository.AnyMemberWithIdAsync(id, currentUserId))
        {
            throw new DomainException(GeneralErrors.Forbidden);
        }

        return direction;
    }

    private void ValidateDirectionToUpdate(Direction direction)
    {
        if (direction.DeletedAt is not null)
        {
            throw new DomainException(DirectionErrors.UpdateDeletedDirection);
        }
    }
}