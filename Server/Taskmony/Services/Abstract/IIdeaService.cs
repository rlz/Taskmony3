using Taskmony.Models.Ideas;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Services.Abstract;

public interface IIdeaService
{
    Task<IEnumerable<Idea>> GetIdeasAsync(Guid[]? id, Guid?[]? directionId, int? offset, int? limit,
        Guid currentUserId);

    Task<IEnumerable<Idea>> GetIdeaByIdsAsync(Guid[] ids);

    Task<Idea> GetIdeaOrThrowAsync(Guid id, Guid currentUserId);

    Task<Idea?> AddIdeaAsync(string description, string? details, Guid? directionId, Generation generation,
        Guid currentUserId);

    Task<Guid?> SetIdeaDescriptionAsync(Guid id, string description, Guid currentUserId);

    Task<Guid?> SetIdeaDetailsAsync(Guid id, string? details, Guid currentUserId);

    Task<Guid?> SetIdeaDirectionAsync(Guid id, Guid? directionId, Guid currentUserId);

    Task<Guid?> SetIdeaDeletedAtAsync(Guid id, DateTime? deletedAtUtc, Guid currentUserId);

    Task<Guid?> SetIdeaGenerationAsync(Guid id, Generation generation, Guid currentUserId);

    Task<Guid?> SetIdeaReviewedAtAsync(Guid id, DateTime? reviewedAtUtc, Guid currentUserId);

    Task SoftDeleteDirectionIdeasAsync(Guid directionId);

    Task UndeleteDirectionIdeasAsync(Guid directionId, DateTime deletedAt);
}