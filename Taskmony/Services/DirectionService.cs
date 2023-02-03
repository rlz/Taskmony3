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

    public DirectionService(IDirectionRepository directionRepository, IUserService userService)
    {
        _directionRepository = directionRepository;
        _userService = userService;
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

        _directionRepository.AddMember(new Membership
        {
            DirectionId = direction.Id,
            UserId = direction.CreatedById
        });

        await _directionRepository.SaveChangesAsync();

        return direction;
    }

    public async Task<bool> AddMember(Guid directionId, Guid memberId, Guid currentUserId)
    {
        var direction = await GetDirectionOrThrowAsync(directionId, currentUserId);

        ValidateDirectionToUpdate(direction);

        var user = await _userService.GetUserOrThrowAsync(memberId);

        _directionRepository.AddMember(new Membership
        {
            DirectionId = direction.Id,
            UserId = user.Id
        });

        return await _directionRepository.SaveChangesAsync();
    }

    public async Task<bool> RemoveMember(Guid directionId, Guid memberId, Guid currentUserId)
    {
        var direction = await GetDirectionOrThrowAsync(directionId, currentUserId);

        ValidateDirectionToUpdate(direction);

        var user = await _userService.GetUserOrThrowAsync(memberId);

        _directionRepository.RemoveMember(new Membership
        {
            DirectionId = direction.Id,
            UserId = user.Id
        });

        return await _directionRepository.SaveChangesAsync();
    }

    public async Task<bool> SetDirectionDeletedAt(Guid id, DateTime? deletedAtUtc, Guid currentUserId)
    {
        var deletedAt = deletedAtUtc is not null ? DeletedAt.From(deletedAtUtc.Value) : null;

        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        if (deletedAtUtc is not null && direction.DeletedAt is not null)
        {
            throw new DomainException(DirectionErrors.AlreadyDeleted);
        }

        direction.DeletedAt = deletedAt;

        return await _directionRepository.SaveChangesAsync();
    }

    public async Task<bool> SetDirectionDetails(Guid id, string? details, Guid currentUserId)
    {
        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        ValidateDirectionToUpdate(direction);

        direction.Details = details;

        return await _directionRepository.SaveChangesAsync();
    }

    public async Task<bool> SetDirectionName(Guid id, string name, Guid currentUserId)
    {
        var newName = DirectionName.From(name);

        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        ValidateDirectionToUpdate(direction);

        direction.Name = newName;

        return await _directionRepository.SaveChangesAsync();
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