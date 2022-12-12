namespace Taskmony.Models.Comments;

public class IdeaComment : Comment
{
    public Guid IdeaId { get; set; }

    public Idea Idea { get; set; }
}