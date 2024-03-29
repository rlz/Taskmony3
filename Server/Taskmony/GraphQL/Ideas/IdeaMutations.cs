using Taskmony.Models.Ideas;
using Taskmony.Services.Abstract;

namespace Taskmony.GraphQL.Ideas;

[ExtendObjectType(typeof(Mutation))]
public class IdeaMutations
{
    public async Task<Idea?> IdeaAdd([Service] IIdeaService ideaService, [GlobalState] Guid currentUserId,
        string description, string? details, Guid? directionId, Generation generation)
    {
        return await ideaService.AddIdeaAsync(description, details, directionId, generation, currentUserId);
    }

    public async Task<Guid?> IdeaSetDescription([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, string description)
    {
        return await ideaService.SetIdeaDescriptionAsync(ideaId, description, currentUserId);
    }

    public async Task<Guid?> IdeaSetDetails([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, string? details)
    {
        return await ideaService.SetIdeaDetailsAsync(ideaId, details, currentUserId);
    }

    public async Task<Guid?> IdeaSetDirection([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, Guid? directionId)
    {
        return await ideaService.SetIdeaDirectionAsync(ideaId, directionId, currentUserId);
    }

    public async Task<Guid?> IdeaSetDeletedAt([Service] IIdeaService ideaService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid ideaId, string? deletedAt)
    {
        DateTime? deletedAtUtc = deletedAt is null ? null : timeConverter.StringToDateTimeUtc(deletedAt);

        return await ideaService.SetIdeaDeletedAtAsync(ideaId, deletedAtUtc, currentUserId);
    }

    public async Task<Guid?> IdeaSetGeneration([Service] IIdeaService ideaService,
        [GlobalState] Guid currentUserId, Guid ideaId, Generation generation)
    {
        return await ideaService.SetIdeaGenerationAsync(ideaId, generation, currentUserId);
    }

    public async Task<Guid?> IdeaSetReviewedAt([Service] IIdeaService ideaService,
        [Service] ITimeConverter timeConverter, [GlobalState] Guid currentUserId, Guid ideaId, string? reviewedAt)
    {
        DateTime? reviewedAtUtc = reviewedAt is null ? null : timeConverter.StringToDateTimeUtc(reviewedAt);

        return await ideaService.SetIdeaReviewedAtAsync(ideaId, reviewedAtUtc, currentUserId);
    }
}