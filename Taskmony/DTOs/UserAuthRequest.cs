using System.ComponentModel.DataAnnotations;

namespace Taskmony.DTOs;

public class UserAuthRequest
{
    [Required]
    public string Login { get; set; }
    
    [Required]
    public string Password { get; set; }
}