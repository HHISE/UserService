using System.ComponentModel.DataAnnotations;
namespace Models;
public class User
{
    [Required]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty; 
    public string Address1 { get; set; } = string.Empty; 
    public string City { get; set; } = string.Empty; 

    [Required]
    public string EmailAddress { get; set; } = string.Empty; 
    public string PhoneNumber { get; set; } = string.Empty; 
}