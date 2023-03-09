namespace Taskmony.Models;

public class MessageTemplate : Entity
{
    public string Name { get; set; } = default!;
    
    public string Subject { get; set; } = default!;

    public string Body { get; set; } = default!;
}