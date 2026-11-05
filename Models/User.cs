using System.ComponentModel.DataAnnotations;
namespace Models;
public class User
{
    [Required]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    public string Address { get; set; } = string.Empty; 
    public string City { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty; 
    public string PhoneNumber { get; set; } = string.Empty; 
}