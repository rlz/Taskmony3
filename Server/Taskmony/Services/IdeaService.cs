using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Ideas;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Services;

public class IdeaService : IIdeaService
{
    private readonly IIdeaRepository _ideaRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly INotificationService _notificationService;
    private readonly ITimeConverter _timeConverter;
    private readonly ICommentRepository _commentRepository;

    public IdeaService(IIdeaRepository ideaRepository, IDirectionRepository directionRepository,
        INotificationService notificationService, ITimeConverter timeConverter, ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
        _ideaRepository = ideaRepository;
        _directionRepository = directionRepository;
        _notificationService = notificationService;
        _timeConverter = timeConverter;
    }

    public async Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[]? directionId, bool? deleted,
        DateTime? lastDeletedAt, int? offset, int? limit, Guid currentUserId)
    {
        int? limitValue = limit == null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset == null ? null : Offset.From(offset.Value).Value;

        //If directionId is [null] return ideas created by the current user with direction id = null
        if (directionId?.Length == 1 && directionId.Contains(null))
        {
            return await _ideaRepository.GetAsync(
                id: id,
                directionId: directionId,
                deleted: deleted ?? false,
                lastDeletedAt: lastDeletedAt,
                offset: offset,
                limit: limit,
                userId: currentUserId);
        }

        var userDirectionIds = await _directionRepository.GetUserDirectionIdsAsync(currentUserId);
        var authorizedDirectionIds = userDirectionIds.Cast<Guid?>().Append(null);

        //If directionId is null return all ideas visible to the current user.
        //That includes ideas from all the directions where user is a member
        //(user is a member of his own directions)

        directionId = directionId == null
            ? authorizedDirectionIds.ToArray()
            : directionId.Intersect(authorizedDirectionIds).ToArray();

        return await _ideaRepository.GetAsync(
            id: id,
            directionId: directionId,
            deleted: deleted ?? false,
            lastDeletedAt: lastDeletedAt,
            offset: offsetValue,
            limit: limitValue,
            userId: currentUserId);
    }

    public async Task<IEnumerable<Idea>> GetIdeaByIdsAsync(Guid[] ids)
    {
        return await _ideaRepository.GetByIdsAsync(ids);
    }

    public async Task<Idea?> AddIdeaAsync(string description, string? details, Guid? directionId, Generation generation,
        Guid currentUserId)
    {
        var idea = new Idea(
            description: Description.From(description),
            details: details,
            createdById: currentUserId,
            generation: generation,
            directionId: directionId);

        if (idea.DirectionId != null &&
            !await _directionRepository.AnyMemberWithIdAsync(idea.DirectionId.Value, idea.CreatedById))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        await _ideaRepository.AddAsync(idea);

        if (!await _ideaRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityAddedAsync(idea, idea.CreatedAt, idea.CreatedById);

        return idea;
    }

    public async Task<Guid?> SetIdeaDescriptionAsync(Guid id, string description, Guid currentUserId)
    {
        var newDescription = Description.From(description);

        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        var oldValue = idea.Description!.Value;
        idea.UpdateDescription(newDescription);

        if (!await _ideaRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(idea, nameof(Idea.Description), oldValue,
            description, currentUserId);

        return idea.Id;
    }

    public async Task<Guid?> SetIdeaDetailsAsync(Guid id, string? details, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        var oldValue = idea.Details;
        idea.UpdateDetails(details);

        if (!await _ideaRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(idea, nameof(Idea.Details), oldValue,
            details, currentUserId);

        return idea.Id;
    }

    public async Task<Guid?> SetIdeaDirectionAsync(Guid id, Guid? directionId, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        if (directionId != null &&
            !await _directionRepository.AnyMemberWithIdAsync(directionId.Value, currentUserId))
        {
            throw new DomainException(DirectionErrors.NotFound);
        }

        var oldDirectionId = idea.DirectionId;
        idea.UpdateDirectionId(directionId);

        if (!await _ideaRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityMovedAsync(idea, oldDirectionId, currentUserId, null);

        return idea.Id;
    }

    public async Task<Guid?> SetIdeaDeletedAtAsync(Guid id, DateTime? deletedAtUtc, Guid currentUserId)
    {
        var deletedAt = deletedAtUtc == null ? null : DeletedAt.From(deletedAtUtc.Value);

        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        var oldValue = idea.DeletedAt?.Value;
        var newValue = deletedAt?.Value;

        idea.UpdateDeletedAt(deletedAt);

        if (!await _ideaRepository.SaveChangesAsync())
        {
            return null;
        }

        if (deletedAt != null)
        {
            await _commentRepository.SoftDeleteIdeaCommentsAsync(new[] {idea.Id});
        }
        else if (oldValue != null)
        {
            await _commentRepository.UndeleteIdeaCommentsAsync(new[] {idea.Id}, oldValue.Value);
        }

        await _notificationService.NotifyDirectionEntityDeletedAtUpdatedAsync(idea, oldValue, newValue,
            currentUserId);

        return idea.Id;
    }

    public async Task<Guid?> SetIdeaGenerationAsync(Guid id, Generation generation, Guid currentUserId)
    {
        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        var oldValue = idea.Generation;
        idea.UpdateGeneration(generation);

        if (!await _ideaRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(idea, nameof(Idea.Generation),
            oldValue?.ToString(),
            generation.ToString(), currentUserId);

        return idea.Id;
    }

    public async Task<Guid?> SetIdeaReviewedAtAsync(Guid id, DateTime? reviewedAtUtc, Guid currentUserId)
    {
        var reviewedAt = reviewedAtUtc == null ? null : ReviewedAt.From(reviewedAtUtc.Value);

        var idea = await GetIdeaOrThrowAsync(id, currentUserId);

        var oldValue = idea.ReviewedAt != null ? _timeConverter.DateTimeToString(idea.ReviewedAt.Value) : null;
        var newValue = reviewedAt != null ? _timeConverter.DateTimeToString(reviewedAt.Value) : null;

        idea.UpdateReviewedAt(reviewedAt);

        if (!await _ideaRepository.SaveChangesAsync())
        {
            return null;
        }

        await _notificationService.NotifyDirectionEntityUpdatedAsync(idea, nameof(Idea.ReviewedAt),
            oldValue, newValue, currentUserId);

        return idea.Id;
    }

    public async Task<Idea> GetIdeaOrThrowAsync(Guid id, Guid currentUserId)
    {
        var idea = await _ideaRepository.GetByIdAsync(id);

        if (idea == null)
        {
            throw new DomainException(IdeaErrors.NotFound);
        }

        //Idea should either be created by the current user or 
        //belong to a direction where the current user is a member
        if (idea.CreatedById != currentUserId && idea.DirectionId == null ||
            idea.DirectionId != null &&
            !await _directionRepository.AnyMemberWithIdAsync(idea.DirectionId.Value, currentUserId))
        {
            throw new DomainException(GeneralErrors.Forbidden);
        }

        return idea;
    }

    public async Task SoftDeleteDirectionIdeasAsync(Guid directionId)
    {
        await _ideaRepository.SoftDeleteDirectionIdeasAndCommentsAsync(directionId);
    }

    public async Task UndeleteDirectionIdeasAsync(Guid directionId, DateTime deletedAt)
    {
        await _ideaRepository.UndeleteDirectionIdeasAndComments(directionId, deletedAt);
    }
}