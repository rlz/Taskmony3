using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Taskmony.Models.Comments;
using Taskmony.Models.Enums;
using Taskmony.Models.Subscriptions;

namespace Taskmony.Models;

public class Idea
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

    public DateTime? DeletedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public Direction? Direction { get; set; }

    public Guid? DirectionId { get; set; }

    [Required]
    public Generation? Generation { get; set; }

    public ICollection<IdeaComment>? Comments { get; set; }

    [NotMapped]
    public ICollection<Notification>? Notifications { get; set; }

    public ICollection<IdeaSubscription>? Subscriptions { get; set; }
}