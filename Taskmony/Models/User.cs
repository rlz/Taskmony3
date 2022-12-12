using System.ComponentModel.DataAnnotations;
using Taskmony.Models.Comments;
using Taskmony.Models.Subscriptions;

namespace Taskmony.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string? Login { get; set; }

    [Required]
    public string? Password { get; set; }

    [Required]
    public string? DisplayName { get; set; }

    [Required]
    public string? Email { get; set; }

    public DateTime? NotificationReadTime { get; set; }

    [Required]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Tasks created by the user
    /// </summary>
    public ICollection<Task>? Tasks { get; set; }

    /// <summary>
    /// Tasks assigned to the user
    /// </summary>
    public ICollection<Task>? AssignedTasks { get; set; }

    public ICollection<Idea>? Ideas { get; set; }

    /// <summary>
    /// Directions in which the user is a member (including own directions?)
    /// </summary>
    public ICollection<Direction>? Directions { get; set; }

    /// <summary>
    /// Directions owned by the user
    /// </summary>
    public ICollection<Direction>? OwnDirections { get; set; }

    public ICollection<Subscription>? Subscriptions { get; set; }

    public ICollection<Comment>? Comments { get; set; }
}