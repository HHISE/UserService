using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models;
public class User
{
    [Required]
    [BsonId]
    public ObjectId MongoId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    public string Address { get; set; } = string.Empty; 
    public string City { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty; 
    public string PhoneNumber { get; set; } = string.Empty; 
}