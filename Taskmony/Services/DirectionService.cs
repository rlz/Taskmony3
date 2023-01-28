using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models;
using Taskmony.Repositories;

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

    public async Task<Direction?> GetDirectionByIdAsync(Guid id)
    {
        return await _directionRepository.GetDirectionByIdAsync(id);
    }

    public async Task<ILookup<Guid, Guid>> GetMemberIdsAsync(Guid[] directionIds, int? offset, int? limit)
    {
        return await _directionRepository.GetMemberIdsAsync(directionIds, offset, limit);
    }

    public async Task<IEnumerable<Direction>> GetDirectionsAsync(Guid[]? id, int? offset, int? limit,
        Guid currentUserId)
    {
        return await _directionRepository.GetDirectionsAsync(id, offset, limit, currentUserId);
    }

    public async Task<IEnumerable<Direction>> GetDirectionsByIdsAsync(Guid[] ids)
    {
        return await _directionRepository.GetDirectionByIdsAsync(ids);
    }

    public async Task<IEnumerable<Guid>> GetUserDirectionIds(Guid userId)
    {
        var userDirections = await _directionRepository.GetDirectionsAsync(null, null, null, userId);
        return userDirections.Select(d => d.Id);
    }

    public async Task<Direction> AddDirection(Direction direction)
    {
        ValidateDirectionName(direction.Name);

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
        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        if (deletedAtUtc is not null && direction.DeletedAt is not null)
        {
            throw new DomainException(DirectionErrors.AlreadyDeleted);
        }

        if (deletedAtUtc is not null && deletedAtUtc > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidDeletedAt);
        }

        direction.DeletedAt = deletedAtUtc;

        return await _directionRepository.SaveChangesAsync();
    }

    public async Task<bool> SetDirectionDetails(Guid id, string? details, Guid currentUserId)
    {
        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        direction.Details = details;

        return await _directionRepository.SaveChangesAsync();
    }

    public async Task<bool> SetDirectionName(Guid id, string name, Guid currentUserId)
    {
        var direction = await GetDirectionOrThrowAsync(id, currentUserId);

        ValidateDirectionName(name);

        direction.Name = name;

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

    private void ValidateDirectionName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(ValidationErrors.InvalidDirectionName);
        }
    }
}