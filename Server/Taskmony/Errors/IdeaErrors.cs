namespace Taskmony.Errors;

public static class IdeaErrors
{
    public static ErrorDetails NotFound => new("Idea not found", "IDEA_NOT_FOUND");

    public static ErrorDetails AlreadyDeleted =>
        new("Idea is already deleted", "IDEA_ALREADY_DELETED");

    public static ErrorDetails UpdateDeletedIdea =>
        new("Cannot change deleted idea", "UPDATE_DELETED_IDEA");

    public static ErrorDetails SubscriptionNotFound =>
        new("Idea subscription not found", "SUBSCRIPTION_NOT_FOUND");

    public static ErrorDetails SubscribeToDeletedIdea =>
        new("Cannot subscribe to deleted idea", "SUBSCRIBE_TO_DELETED_IDEA");

    public static ErrorDetails AlreadySubscribedToIdea =>
        new("User is already subscribed to the specified idea", "ALREADY_SUBSCRIBED_TO_IDEA");
}