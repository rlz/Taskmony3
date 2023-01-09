using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Taskmony.Models.Enums;

namespace Taskmony.Models.Notifications;

public class Notification
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime? ModifiedAt { get; set; }

    public Guid ActorId { get; set; }

    [Required]
    public User? Actor { get; set; }

    [Required]
    public NotifiableType? NotifiableType { get; set; }

    public Guid NotifiableId { get; set; }

    [Required]
    public ActionType? ActionType { get; set; }

    public ActionItemType? ActionItemType { get; set; }
    
    [NotMapped]
    public IActionItem ActionItem { get; set; }

    public Guid? ActionItemId { get; set; }

    public string? Field { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }
}