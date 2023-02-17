using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;
using Taskmony.ValueObjects;

namespace Taskmony.Services;

public class IdeaService : IIdeaService
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IDirectionService _directionService;
    private readonly INotificationService _notificationService;
    private readonly ITimeConverter _timeConverter;

    public IdeaService(IIdeaRepository ideaRepository, IDirectionService directionService,
        INotificationService notificationService, ITimeConverter timeConverter)
    {
        _ideaRepository = ideaRepository;
        _directionService = directionService;
        _notificationService = notificationService;
        _timeConverter = timeConverter;
    }

    public async Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[]? directionId, int? offset,
        int? limit, Guid currentUserId)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        //If directionId is [null] return ideas created by the current user with direction id = null
        if (directionId?.Length == 1 && directionId.Contains(null))
        {
            return await _ideaRepository.GetIdeasAsync(id, directionId, offset, limit, currentUserId);
        }

        var userDirectionIds = await _directionService.GetUserDirectionIds(currentUserId);
        var authorizedDirectionIds = userDirectionIds.Cast<Guid?>().Append(null);

        //If directionId is null return all ideas visible to the current user.
        //That includes ideas from all the directions where user is a member
        //(user is a member of his own directions)

        directionId = directionId == null
            ? authorizedDirectionIds.ToArray()
            : directionId.Intersect(authorizedDirectionIds).ToArray();

        return await _ideaRepository.GetIdeasAsync(id, directionId, offsetValue, limitValue, currentUserId);
    }

    public async Task<IEnumerable<Idea>> GetIdeaByIdsAsync(Guid[] ids)
    {
        return await _ideaRepository.GetIdeaByIdsAsync(ids);
    }

    public async Task<Idea?> AddIdeaAsync(Idea idea)
    {
        if (idea.DirectionId is not null &&
            !await _directionService.AnyMemberWithIdAsync(idea.DirectionId.Value, idea.CreatedById))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        await _ideaRepository.AddIdeaAsync(idea);

        if (!await _ideaRepository.SaveChangesAsync())
        {
            return null;
        }

        if (idea.DirectionId is not null)
        {
            await _notificationService.NotifyItemAddedAsync(NotifiableType.Direction, idea.DirectionId.Value,
                idea.ActionItemType, idea.Id, idea.CreatedById, idea.CreatedAt);
        }

        return idea;
    }

    public async Task<Guid?> SetIdeaDescriptionAsync(Guid id, string description, Guid currentUserId)
    {
        var newDescription = Description.From(description);

        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        ValidateIdeaToUpdate(idea);

        var oldValue = idea.Description!.Value;
        idea.Description = newDescription;

        return await SaveChangesAndNotifyAsync(idea, nameof(Idea.Description), oldValue, description, currentUserId);
    }

    public async Task<Guid?> SetIdeaDetailsAsync(Guid id, string? details, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        ValidateIdeaToUpdate(idea);

        var oldValue = idea.Details;
        idea.Details = details;

        return await SaveChangesAndNotifyAsync(idea, nameof(Idea.Details), oldValue, idea.Details, currentUserId);
    }

    public async Task<Guid?> SetIdeaDirectionAsync(Guid id, Guid? directionId, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        ValidateIdeaToUpdate(idea);

        if (directionId is not null &&
            !await _directionService.AnyMemberWithIdAsync(directionId.Value, currentUserId))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        var oldDirectionId = idea.DirectionId;
        idea.DirectionId = directionId;

        if (!await _ideaRepository.SaveChangesAsync())
        {
            return null;
        }

        if (idea.DirectionId is not null)
        {
            await _notificationService.NotifyItemAddedAsync(NotifiableType.Direction, idea.DirectionId.Value,
                idea.ActionItemType, idea.Id, currentUserId, null);
        }

        if (oldDirectionId is not null)
        {
            await _notificationService.NotifyItemRemovedAsync(NotifiableType.Direction, oldDirectionId.Value,
                idea.ActionItemType, idea.Id, currentUserId, null);
        }

        return idea.Id;
    }

    public async Task<Guid?> SetIdeaDeletedAtAsync(Guid id, DateTime? deletedAtUtc, Guid currentUserId)
    {
        var deletedAt = deletedAtUtc is null ? null : DeletedAt.From(deletedAtUtc.Value);

        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        if (idea.DeletedAt is not null && deletedAtUtc is not null)
        {
            throw new DomainException(IdeaErrors.AlreadyDeleted);
        }

        if (deletedAtUtc is not null && deletedAtUtc > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidDeletedAt);
        }

        var oldValue = idea.DeletedAt?.Value;
        idea.DeletedAt = deletedAt;

        if (!await _ideaRepository.SaveChangesAsync())
        {
            return null;
        }

        if (idea.DeletedAt is not null && idea.DirectionId is not null)
        {
            await _notificationService.NotifyItemRemovedAsync(NotifiableType.Direction, idea.DirectionId.Value,
                ActionItemType.Idea, idea.Id, currentUserId, idea.DeletedAt.Value);
        }
        else if (idea.DirectionId is not null)
        {
            var newValue = idea.DeletedAt is null ? null : _timeConverter.DateTimeToString(idea.DeletedAt.Value);
            var oldValueString = oldValue is null ? null : _timeConverter.DateTimeToString(oldValue.Value);

            await _notificationService.NotifyItemUpdatedAsync(NotifiableType.Idea, idea.Id, currentUserId,
                nameof(Idea.DeletedAt), oldValueString, newValue);
        }

        return idea.Id;
    }

    public async Task<Guid?> SetIdeaGenerationAsync(Guid id, Generation generation, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        ValidateIdeaToUpdate(idea);

        var oldValue = idea.Generation;
        idea.Generation = generation;

        return await SaveChangesAndNotifyAsync(idea, nameof(Idea.Generation), oldValue?.ToString(),
            generation.ToString(), currentUserId);
    }

    public async Task<Guid?> SetIdeaReviewedAtAsync(Guid id, DateTime? reviewedAtUtc, Guid currentUserId)
    {
        var reviewedAt = reviewedAtUtc is null ? null : ReviewedAt.From(reviewedAtUtc.Value);

        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        ValidateIdeaToUpdate(idea);

        var oldValue = idea.ReviewedAt;
        idea.ReviewedAt = reviewedAt;

        return await SaveChangesAndNotifyAsync(idea, nameof(Idea.ReviewedAt), oldValue?.ToString(),
            reviewedAt?.ToString(), currentUserId);
    }

    private async Task<Guid?> SaveChangesAndNotifyAsync(Idea idea, string field, string? oldValue, string? newValue,
        Guid currentUserId)
    {
        if (!await _ideaRepository.SaveChangesAsync())
        {
            return null;
        }

        if (idea.DirectionId is not null)
        {
            await _notificationService.NotifyItemUpdatedAsync(NotifiableType.Task, idea.Id, currentUserId, field,
                oldValue, newValue);
        }

        return idea.Id;
    }

    public async Task<Idea> GetIdeaOrThrowAsync(Guid id, Guid currentUserId)
    {
        var idea = await _ideaRepository.GetIdeaByIdAsync(id);

        //Idea should either be created by the current user or 
        //belong to a direction where the current user is a member
        if (idea is null ||
            idea.CreatedById != currentUserId && idea.DirectionId == null ||
            idea.DirectionId != null &&
            !await _directionService.AnyMemberWithIdAsync(idea.DirectionId.Value, currentUserId))
        {
            throw new DomainException(IdeaErrors.NotFound);
        }

        return idea;
    }

    private void ValidateIdeaToUpdate(Idea idea)
    {
        if (idea.DeletedAt is not null)
        {
            throw new DomainException(IdeaErrors.UpdateDeletedIdea);
        }
    }
}