using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Taskmony.Models.Comments;
using Taskmony.Models.Enums;
using Taskmony.Models.Subscriptions;

namespace Taskmony.Models;

public class Task
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string? Description { get; set; }

    public string? Details { get; set; }

    [Required]
    public User? CreatedBy { get; set; }

    [Required]
    public Guid? CreatedById { get; set; }

    [Required]
    public DateTime? CreatedAt { get; set; }

    [Required]
    public DateTime? StartAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Direction? Direction { get; set; }

    public Guid? DirectionId { get; set; }

    public User? Assignee { get; set; }

    public Guid? AssigneeId { get; set; }

    public RepeatMode? RepeatMode { get; set; }

    public int? RepeatEvery { get; set; }

    public Guid? GroupId { get; set; }

    public ICollection<TaskComment>? Comments { get; set; }

    [NotMapped]
    public ICollection<Notification>? Notifications { get; set; }

    public ICollection<TaskSubscription>? Subscriptions { get; set; }
}