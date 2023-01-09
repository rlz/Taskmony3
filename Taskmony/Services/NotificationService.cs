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
            var user = await _userService.GetUsersAsync(new[] {actionItemId}, null, null, null, null, currentUserId);
            return user.FirstOrDefault();
        }

        if (actionItemType == ActionItemType.Task)
        {
            var task = await _taskService.GetTasksAsync(new[] {actionItemId}, null, null, null, currentUserId);
            return task.FirstOrDefault();
        }

        if (actionItemType == ActionItemType.Idea)
        {
            throw new NotImplementedException();
        }

        if (actionItemType == ActionItemType.Comment)
        {
            throw new NotImplementedException();
        }

        return null;
    }

    public async Task<IEnumerable<Notification>> GetTaskNotificationsAsync(Guid taskId, DateTime? start, DateTime? end)
    {
        return await _notificationRepository.GetNotificationsAsync(NotifiableType.Task, taskId, start, end);
    }

    public async Task<IEnumerable<Notification>> GetIdeaNotificationsAsync(Guid ideaId, DateTime? start, DateTime? end)
    {
        return await _notificationRepository.GetNotificationsAsync(NotifiableType.Idea, ideaId, start, end);
    }

    public async Task<IEnumerable<Notification>> GetDirectionNotificationsAsync(Guid directionId, DateTime? start,
        DateTime? end)
    {
        return await _notificationRepository.GetNotificationsAsync(NotifiableType.Direction, directionId, start, end);
    }
}