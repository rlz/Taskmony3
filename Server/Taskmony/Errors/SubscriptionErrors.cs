namespace Taskmony.Errors;

public class SubscriptionErrors
{
    public static ErrorDetails NotFound =>
        new("Subscription not found", "SUBSCRIPTION_NOT_FOUND");

    public static ErrorDetails SubscribeToDeletedEntity =>
        new("Cannot subscribe to deleted entity", "SUBSCRIBE_TO_DELETED_ENTITY");

    public static ErrorDetails SubscribeToCompletedTask =>
        new("Cannot subscribe to completed task", "SUBSCRIBE_TO_COMPLETED_TASK");

     public static ErrorDetails AlreadySubscribed =>
        new("User is already subscribed", "ALREADY_SUBSCRIBED");
}