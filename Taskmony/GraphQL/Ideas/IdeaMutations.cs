using HotChocolate.AspNetCore.Authorization;
using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.Services;

namespace Taskmony.GraphQL.Tasks;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class IdeaMutations
{
    [Authorize]
    public async Task<Idea?> IdeaAdd([Service] IIdeaService ideaService, [GlobalState] Guid userId,
        string description, string? details, Guid? directionId, Generation generation)
    {
        var idea = new Idea
        {
            CreatedById = userId,
            Description = description,
            Details = details,
            DirectionId = directionId,
            Generation = generation
        };

        return await ideaService.AddIdea(idea);
    }

    [Authorize]
    public async Task<Guid?> IdeaSetDescription([Service] IIdeaService ideaService,
        [GlobalState] Guid userId, Guid ideaId, string description)
    {
        if (await ideaService.SetIdeaDescription(ideaId, description, userId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetDetails([Service] IIdeaService ideaService,
        [GlobalState] Guid userId, Guid ideaId, string? details)
    {
        if (await ideaService.SetIdeaDetails(ideaId, details, userId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetDirection([Service] IIdeaService ideaService,
        [GlobalState] Guid userId, Guid ideaId, Guid? directionId)
    {
        if (await ideaService.SetIdeaDirection(ideaId, directionId, userId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetDeletedAt([Service] IIdeaService ideaService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid userId, Guid ideaId, string? deletedAt)
    {
        DateTime? deletedAtUtc = deletedAt is null ? null : timeConverter.StringToDateTimeUtc(deletedAt);

        if (await ideaService.SetIdeaDeletedAt(ideaId, deletedAtUtc, userId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetGeneration([Service] IIdeaService ideaService,
        [GlobalState] Guid userId, Guid ideaId, Generation generation)
    {
        if (await ideaService.SetIdeaGeneration(ideaId, generation, userId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetReviewedAt([Service] IIdeaService ideaService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid userId, Guid ideaId, string? reviewedAt)
    {
        DateTime? reviewedAtUtc = reviewedAt is null ? null : timeConverter.StringToDateTimeUtc(reviewedAt);

        if (await ideaService.SetIdeaReviewedAt(ideaId, reviewedAtUtc, userId))
        {
            return ideaId;
        }

        return null;
    }
}