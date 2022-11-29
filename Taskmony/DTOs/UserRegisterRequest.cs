using System.ComponentModel.DataAnnotations;

namespace Taskmony.DTOs;

public class UserRegisterRequest
{
    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }
    
    [Required]
    public string DisplayName { get; set; }
    
    [Required]
    public string Email { get; set; }
}