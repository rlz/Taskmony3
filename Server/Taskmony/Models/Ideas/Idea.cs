using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Comments;
using Taskmony.Models.Directions;
using Taskmony.Models.Notifications;
using Taskmony.Models.Subscriptions;
using Taskmony.Models.Users;
using Taskmony.Models.ValueObjects;

namespace Taskmony.Models.Ideas;

public class Idea : DirectionEntity
{
    public override ActionItemType ActionItemType => ActionItemType.Idea;

    public Description? Description { get; private set; }

    public Details? Details { get; private set; }

    public User? CreatedBy { get; private set; }

    public Guid CreatedById { get; private set; }

    public DateTime? CreatedAt { get; private set; }

    public DeletedAt? DeletedAt { get; private set; }

    public ReviewedAt? ReviewedAt { get; private set; }

    public Generation? Generation { get; private set; }

    public ICollection<IdeaComment>? Comments { get; private set; }

    public ICollection<Notification>? Notifications { get; private set; }

    public ICollection<IdeaSubscription>? Subscriptions { get; private set; }

    // Required by EF Core
    private Idea()
    {
    }

    public Idea(Description description, Details details, Guid createdById, Generation generation, Guid? directionId,
        ReviewedAt? reviewedAt = null, DateTime? createdAt = null, DeletedAt? deletedAt = null)
    {
        Description = description;
        Details = details;
        CreatedById = createdById;
        Generation = generation;
        ReviewedAt = reviewedAt;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        DeletedAt = deletedAt;
        DirectionId = directionId;
    }

    public Idea(Guid id, Description description, Details details, Guid createdById, Generation generation,
        Guid? directionId, ReviewedAt? reviewedAt = null, DateTime? createdAt = null, DeletedAt? deletedAt = null) :
        this(description, details, createdById, generation, directionId, reviewedAt, createdAt, deletedAt)
    {
        Id = id;
    }

    public void ValidateIdeaToUpdate()
    {
        if (DeletedAt != null)
        {
            throw new DomainException(IdeaErrors.UpdateDeletedIdea);
        }
    }

    public void UpdateDescription(Description description)
    {
        ValidateIdeaToUpdate();

        Description = description;
    }

    public void UpdateDetails(Details details)
    {
        ValidateIdeaToUpdate();

        Details = details;
    }

    public void UpdateReviewedAt(ReviewedAt? reviewedAt)
    {
        ValidateIdeaToUpdate();

        ReviewedAt = reviewedAt;
    }

    public void UpdateDeletedAt(DeletedAt? deletedAt)
    {
        if (deletedAt != null && DeletedAt != null)
        {
            throw new DomainException(IdeaErrors.AlreadyDeleted);
        }

        DeletedAt = deletedAt;
    }

    public void UpdateGeneration(Generation generation)
    {
        ValidateIdeaToUpdate();

        Generation = generation;
    }

    public void UpdateDirectionId(Guid? directionId)
    {
        ValidateIdeaToUpdate();

        DirectionId = directionId;
    }
}