namespace Taskmony.Errors;

public static class TaskErrors
{
    public static ErrorDetails AssignPrivateTask =>
        new("Cannot assign task with no direction", "ASSIGN_INVALID_TASK");
    
    public static ErrorDetails NotFound => new("Task not found", "TASK_NOT_FOUND");

    public static ErrorDetails AlreadyDeleted =>
        new("Task is already deleted", "TASK_ALREADY_DELETED");

    public static ErrorDetails CompleteDeletedTask =>
        new("Cannot complete deleted task", "COMPLETE_DELETED_TASK");

    public static ErrorDetails AlreadyCompleted =>
        new("Task is already completed", "TASK_ALREADY_COMPLETED");

    public static ErrorDetails UpdateCompletedOrDeletedTask =>
        new("Cannot change completed or deleted task", "UPDATE_COMPLETED_OR_DELETED_TASK");

    public static ErrorDetails GroupWithoutRecurrence =>
        new("Cannot add non-recurring task to group", "GROUP_WITHOUT_RECURRENCE");
}