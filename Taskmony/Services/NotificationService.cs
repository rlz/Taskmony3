using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;
using Taskmony.Repositories;

namespace Taskmony.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserService _userService;
    private readonly ITaskService _taskService;
    private readonly IIdeaService _ideaService;
    private readonly ICommentService _commentService;

    public NotificationService(INotificationRepository notificationRepository, ICommentService commentService,
        IIdeaService ideaService, ITaskService taskService, IUserService userService)
    {
        _notificationRepository = notificationRepository;
        _commentService = commentService;
        _ideaService = ideaService;
        _taskService = taskService;
        _userService = userService;
    }

    public async Task<IActionItem?> GetActionItemAsync(ActionItemType actionItemType, Guid actionItemId,
        Guid currentUserId)
    {
        if (actionItemType == ActionItemType.User)
        {
            var user = await _userService.GetUsersAsync(new[] { actionItemId }, null, null, null, null, currentUserId);
            return user.FirstOrDefault();
        }

        if (actionItemType == ActionItemType.Task)
        {
            var task = await _taskService.GetTasksAsync(new[] { actionItemId }, null, null, null, currentUserId);
            return task.FirstOrDefault();
        }

        if (actionItemType == ActionItemType.Idea)
        {
            var idea = await _ideaService.GetIdeasAsync(new[] { actionItemId }, null, null, null, currentUserId);
            return idea.FirstOrDefault();
        }

        if (actionItemType == ActionItemType.Comment)
        {
            return await _commentService.GetCommentById(actionItemId);
        }

        return null;
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByNotifiableIdAsync(Guid id, DateTime? start,
        DateTime? end)
    {
        return await _notificationRepository.GetNotificationsAsync(id, start, end);
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByNotifiableIdsAsync(Guid[] ids, DateTime? start,
        DateTime? end)
    {
        return await _notificationRepository.GetNotificationsAsync(ids, start, end);
    }
}