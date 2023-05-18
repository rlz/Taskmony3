using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Comments;
using Taskmony.Models.Directions;
using Taskmony.Models.Ideas;
using Taskmony.Models.Notifications;
using Taskmony.Models.Subscriptions;
using Taskmony.Models.Tasks;
using Taskmony.Models.ValueObjects;
using Task = Taskmony.Models.Tasks.Task;

namespace Taskmony.Models.Users;

public class User : Entity, IActionItem
{
    public ActionItemType ActionItemType => ActionItemType.User;

    public Login? Login { get; private set; }

    public string? Password { get; private set; }

    public DisplayName? DisplayName { get; private set; }

    public Email? Email { get; private set; }

    public DateTime? NotificationReadTime { get; private set; }

    public DateTime? CreatedAt { get; private set; }

    /// <summary>
    /// Tasks created by the user
    /// </summary>
    public ICollection<Task>? Tasks { get; private set; }

    /// <summary>
    /// Tasks assigned to the user
    /// </summary>
    public ICollection<Assignment>? AssignedTo { get; private set; }

    /// <summary>
    /// Tasks assigned by the user
    /// </summary>
    public ICollection<Assignment>? AssignedBy { get; private set; }

    public ICollection<Idea>? Ideas { get; private set; }

    /// <summary>
    /// Directions in which the user is a member
    /// </summary>
    public ICollection<Direction>? Directions { get; private set; }

    /// <summary>
    /// Directions owned by the user
    /// </summary>
    public ICollection<Direction>? OwnDirections { get; private set; }

    public ICollection<Subscription>? Subscriptions { get; private set; }

    public ICollection<Comment>? Comments { get; private set; }

    public ICollection<RefreshToken>? RefreshTokens { get; private set; }

    // Required by EF Core
    private User()
    {
    }

    public User(Login login, DisplayName displayName, Email email, string passwordHash, DateTime? createdAt = null)
    {
        Login = login;
        Password = passwordHash;
        DisplayName = displayName;
        Email = email;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }

    public User(Guid id, Login login, DisplayName displayName, Email? email, DateTime? notificationReadTime)
    {
        Id = id;
        Login = login;
        DisplayName = displayName;
        Email = email;
        NotificationReadTime = notificationReadTime;
    }

    public void UpdateLogin(Login login)
    {
        Login = login;
    }

    public void UpdateDisplayName(DisplayName displayName)
    {
        DisplayName = displayName;
    }

    public void UpdateNotificationReadTime(DateTime? notificationReadTime)
    {
        if (notificationReadTime > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidNotificationReadTime);
        }

        NotificationReadTime = notificationReadTime;
    }

    public void UpdateEmail(Email email)
    {
        Email = email;
    }

    public void UpdatePassword(string passwordHash)
    {
        Password = passwordHash;
    }
}