using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Taskmony.Models.Notifications;

namespace Taskmony.Models;

public class Direction
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string? Name { get; set; }

    public string? Details { get; set; }

    [Required]
    public User? CreatedBy { get; set; }

    public Guid CreatedById { get; set; }

    [Required]
    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<User>? Members { get; set; }

    public ICollection<Task>? Tasks { get; set; }

    public ICollection<Idea>? Ideas { get; set; }

    [NotMapped]
    public ICollection<Notification>? Notifications { get; set; }
}