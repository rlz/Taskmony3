namespace Taskmony.Errors;

public static class TaskErrors
{
    public static ErrorDetails InvalidAssignee =>
        new("Cannot assign task to the specified user", "INVALID_ASSIGNEE");

    public static ErrorDetails DirectionIsMissing =>
        new("Direction must be specified", "DIRECTION_IS_MISSING");

    public static ErrorDetails NotFound => new("Task not found", "TASK_NOT_FOUND");

    public static ErrorDetails AlreadyDeleted =>
        new("Task is already deleted", "TASK_ALREADY_DELETED");

    public static ErrorDetails DeleteFutureTask =>
        new("Cannot delete task that has not started yet", "DELETE_FUTURE_TASK");

    public static ErrorDetails CompleteFutureTask =>
        new("Cannot complete task that has not started yet", "COMPLETE_FUTURE_TASK");

    public static ErrorDetails CompleteDeletedTask =>
        new("Cannot complete deleted task", "COMPLETE_DELETED_TASK");

    public static ErrorDetails AlreadyCompleted =>
        new("Task is already completed", "TASK_ALREADY_COMPLETED");

    public static ErrorDetails RepeatEveryIsMissing => 
        new("Repeat every must be specified if repeat mode is custom", "REPEAT_EVERY_IS_MISSING");

    public static ErrorDetails UpdateCompletedOrDeletedTask =>
        new("Cannot change completed or deleted task", "UPDATE_COMPLETED_OR_DELETED_TASK");
}