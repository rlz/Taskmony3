using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.Repositories;

namespace Taskmony.Services;

public class IdeaService : IIdeaService
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IDirectionService _directionService;

    public IdeaService(IIdeaRepository ideaRepository, IDirectionService directionService)
    {
        _ideaRepository = ideaRepository;
        _directionService = directionService;
    }

    public async Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[]? directionId, int? offset,
        int? limit, Guid currentUserId)
    {
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

        return await _ideaRepository.GetIdeasAsync(id, directionId, offset, limit, currentUserId);
    }

    public async Task<IEnumerable<Idea>> GetIdeaByIdsAsync(Guid[] ids)
    {
        return await _ideaRepository.GetIdeaByIdsAsync(ids);
    }

    public async Task<Idea> AddIdeaAsync(Idea idea)
    {
        if (idea.DirectionId is not null &&
            !await _directionService.AnyMemberWithId(idea.DirectionId.Value, idea.CreatedById))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        await _ideaRepository.AddIdeaAsync(idea);

        await _ideaRepository.SaveChangesAsync();

        return idea;
    }

    public async Task<bool> SetIdeaDescriptionAsync(Guid id, string description, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        if (idea.DeletedAt is not null)
        {
            throw new DomainException(IdeaErrors.UpdateDeletedIdea);
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException(ValidationErrors.InvalidDescription);
        }

        idea.Description = description;

        return await _ideaRepository.SaveChangesAsync();
    }

    public async Task<bool> SetIdeaDetailsAsync(Guid id, string? details, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        if (idea.DeletedAt is not null)
        {
            throw new DomainException(IdeaErrors.UpdateDeletedIdea);
        }

        idea.Details = details;

        return await _ideaRepository.SaveChangesAsync();
    }

    public async Task<bool> SetIdeaDirectionAsync(Guid id, Guid? directionId, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        if (idea.DeletedAt is not null)
        {
            throw new DomainException(IdeaErrors.UpdateDeletedIdea);
        }

        if (directionId is not null &&
            !await _directionService.AnyMemberWithId(directionId.Value, currentUserId))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        idea.DirectionId = directionId;

        return await _ideaRepository.SaveChangesAsync();
    }

    public async Task<bool> SetIdeaDeletedAtAsync(Guid id, DateTime? deletedAtUtc, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        if (idea.DeletedAt is not null && deletedAtUtc is not null)
        {
            throw new DomainException(IdeaErrors.AlreadyDeleted);
        }

        if (deletedAtUtc is not null && deletedAtUtc > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidDeletedAt);
        }

        idea.DeletedAt = deletedAtUtc;

        return await _ideaRepository.SaveChangesAsync();
    }

    public async Task<bool> SetIdeaGenerationAsync(Guid id, Generation generation, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        if (idea.DeletedAt is not null)
        {
            throw new DomainException(IdeaErrors.UpdateDeletedIdea);
        }

        idea.Generation = generation;

        return await _ideaRepository.SaveChangesAsync();
    }

    public async Task<bool> SetIdeaReviewedAtAsync(Guid id, DateTime? reviewedAtUtc, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        if (idea.DeletedAt is not null)
        {
            throw new DomainException(IdeaErrors.UpdateDeletedIdea);
        }

        if (reviewedAtUtc is not null && reviewedAtUtc > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidReviewedAt);
        }

        idea.ReviewedAt = reviewedAtUtc;

        return await _ideaRepository.SaveChangesAsync();
    }

    public async Task<Idea> GetIdeaOrThrowAsync(Guid id, Guid currentUserId)
    {
        var idea = await _ideaRepository.GetIdeaByIdAsync(id);

        //Idea should either be created by the current user or 
        //belong to a direction where the current user is a member
        if (idea is null ||
            idea.CreatedById != currentUserId && idea.DirectionId == null ||
            idea.DirectionId != null && !await _directionService.AnyMemberWithId(idea.DirectionId.Value, currentUserId))
        {
            throw new DomainException(IdeaErrors.NotFound);
        }

        return idea;
    }
}