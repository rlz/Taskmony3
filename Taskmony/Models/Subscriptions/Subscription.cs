using System.ComponentModel.DataAnnotations;

namespace Taskmony.Models.Subscriptions;

public abstract class Subscription
{
    [Key] 
    public Guid Id { get; set; }

    [Required] 
    public User? User { get; set; }

    public Guid UserId { get; set; }

    [Required] 
    public DateTime? SubscribedAt { get; set; }
}