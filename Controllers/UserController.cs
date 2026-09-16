using Microsoft.AspNetCore.Mvc;
using Models;
namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }
    
    private static readonly User[] Users =
    [
      new User
{
    Id = Guid.NewGuid(),
    Name = "Helle",
    Address1 = "Aarhusvej 10",
    PostalCode = 8000,
    City = "Aarhus",
    EmailAddress = "helle@email.com"
},

new User
{
    Id = Guid.NewGuid(),
    Name = "Sara",
    Address1 = "Viborgvej 1",
    PostalCode = 8800,
    City = "Viborg",
    EmailAddress = "sara@email.com"
},
     
new User
{
    Id = Guid.NewGuid(),
    Name = "Julie",
    Address1 = "Hobrovej 8",
    PostalCode = 8000,
    City = "Aarhus",
    EmailAddress = "ali@email.com"
}
    ];
  

// Hent alle brugere
[HttpGet]
public IEnumerable<User> GetAll()
{
    return Users;
}

// Hent én bestemt bruger
[HttpGet("{userId}", Name = "GetUserById")]
public User? Get(Guid userId)
{
    foreach (User user in Users)
    {
        if (user.Id == userId)
        {
            return user;
        }
    }

    return null;
}

}