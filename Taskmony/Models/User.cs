using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taskmony.Models;

public class User
{
    [Key]
    [GraphQLType(typeof(IdType))]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }
    
    [Required]
    public string DisplayName { get; set; }
    
    [Required]
    public string Email { get; set; }
    
    [GraphQLType(typeof(StringType))]
    public DateTime? NotificationReadTime { get; set; }
}