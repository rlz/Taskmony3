namespace Taskmony.Models;

public class Membership
{
    public Guid DirectionId { get; set; }
    
    public Guid UserId { get; set; }
    
    public DateTime CreatedAt { get; set; }
}