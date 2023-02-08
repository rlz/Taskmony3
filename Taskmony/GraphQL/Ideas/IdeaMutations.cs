using HotChocolate.AspNetCore.Authorization;
using Taskmony.Models;
using Taskmony.Models.Enums;
using Taskmony.Services;
using Taskmony.Services.Abstract;
using Taskmony.ValueObjects;

namespace Taskmony.GraphQL.Ideas;

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
            Description = Description.From(description),
            Details = details,
            DirectionId = directionId,
            Generation = generation
        };

        return await ideaService.AddIdeaAsync(idea);
    }

    [Authorize]
    public async Task<Guid?> IdeaSetDescription([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, string description)
    {
        if (await ideaService.SetIdeaDescriptionAsync(ideaId, description, currentUserId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetDetails([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, string? details)
    {
        if (await ideaService.SetIdeaDetailsAsync(ideaId, details, currentUserId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetDirection([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, Guid? directionId)
    {
        if (await ideaService.SetIdeaDirectionAsync(ideaId, directionId, currentUserId))
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

        if (await ideaService.SetIdeaDeletedAtAsync(ideaId, deletedAtUtc, currentUserId))
        {
            return ideaId;
        }

        return null;
    }

    [Authorize]
    public async Task<Guid?> IdeaSetGeneration([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, Generation generation)
    {
        if (await ideaService.SetIdeaGenerationAsync(ideaId, generation, currentUserId))
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

        if (await ideaService.SetIdeaReviewedAtAsync(ideaId, reviewedAtUtc, currentUserId))
        {
            return ideaId;
        }

        return null;
    }
}