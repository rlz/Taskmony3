using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Taskmony.Models.Enums;
using Taskmony.Models.Notifications;

namespace Taskmony.Models.Comments;

public abstract class Comment : IActionItem
{
    [Key]
    public Guid Id { get; set; }

    [NotMapped]
    public ActionItemType ActionItemType => ActionItemType.Comment;

    [Required]
    public string? Text { get; set; }

    [Required]
    public DateTime? CreatedAt { get; set; }

    public Guid CreatedById { get; set; }

    [Required]
    public User? CreatedBy { get; set; }
}