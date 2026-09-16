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
            Name = "Ali",
            EmailAddress = "ali@email.com"
        },

        new User
        {
            Id = Guid.NewGuid(),
            Name = "Sara",
            EmailAddress = "sara@email.com"
        },

        new User
        {
            Id = Guid.NewGuid(),
            Name = "Omar",
            EmailAddress = "omar@email.com"
        }
    ];
  

[HttpGet("{userId}", Name = "GetUserById")]
public User Get(Guid userId)
{
    return Users.First(user => user.Id == userId);
}

}