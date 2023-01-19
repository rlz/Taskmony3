using HotChocolate.AspNetCore.Authorization;
using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.Services;

namespace Taskmony.GraphQL.Tasks;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class IdeaMutations
{
    [Authorize]
    public async Task<Idea?> IdeaAdd([Service] IIdeaService ideaService, [GlobalState] Guid currentUserId,
        string description, string? details, Guid? directionId, Generation generation)
    {
        var idea = new Idea
        {
            CreatedById = currentUserId,
            Description = description,
            Details = details,
            DirectionId = directionId,
            Generation = generation
        };

        return await ideaService.AddIdea(idea);
    }

    [Authorize]
    public async Task<Guid?> IdeaSetDescription([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, string description)
    {
        if (await ideaService.SetIdeaDescription(ideaId, description, currentUserId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetDetails([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, string? details)
    {
        if (await ideaService.SetIdeaDetails(ideaId, details, currentUserId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetDirection([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, Guid? directionId)
    {
        if (await ideaService.SetIdeaDirection(ideaId, directionId, currentUserId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetDeletedAt([Service] IIdeaService ideaService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid ideaId, string? deletedAt)
    {
        DateTime? deletedAtUtc = deletedAt is null ? null : timeConverter.StringToDateTimeUtc(deletedAt);

        if (await ideaService.SetIdeaDeletedAt(ideaId, deletedAtUtc, currentUserId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetGeneration([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, Generation generation)
    {
        if (await ideaService.SetIdeaGeneration(ideaId, generation, currentUserId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetReviewedAt([Service] IIdeaService ideaService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid ideaId, string? reviewedAt)
    {
        DateTime? reviewedAtUtc = reviewedAt is null ? null : timeConverter.StringToDateTimeUtc(reviewedAt);

        if (await ideaService.SetIdeaReviewedAt(ideaId, reviewedAtUtc, currentUserId))
        {
            return ideaId;
        }

        return null;
    }
}