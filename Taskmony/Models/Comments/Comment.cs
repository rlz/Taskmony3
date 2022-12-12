using System.ComponentModel.DataAnnotations;

namespace Taskmony.Models.Comments;

public abstract class Comment
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string? Text { get; set; }

    [Required]
    public DateTime? CreatedAt { get; set; }

    [Required]
    public Guid? CreatedById { get; set; }

    [Required]
    public User? CreatedBy { get; set; }
}