using Taskmony.Models.Ideas;
using Taskmony.ValueObjects;

namespace Taskmony.Models.Comments;

public class IdeaComment : Comment
{
    public Guid IdeaId { get; private set; }

    public Idea Idea { get; private set; } = default!;

    // Required by EF Core
    private IdeaComment()
    {
    }

    public IdeaComment(CommentText text, Guid createdById, Guid ideaId, DateTime? createdAt = null,
        DeletedAt? deletedAt = null) : base(text, createdById, createdAt, deletedAt)
    {
        IdeaId = ideaId;
    }
}