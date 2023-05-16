namespace Taskmony.Errors;

public static class IdeaErrors
{
    public static ErrorDetails NotFound => new("Idea not found", "IDEA_NOT_FOUND");

    public static ErrorDetails AlreadyDeleted =>
        new("Idea is already deleted", "IDEA_ALREADY_DELETED");

    public static ErrorDetails UpdateDeletedIdea =>
        new("Cannot change deleted idea", "UPDATE_DELETED_IDEA");
}