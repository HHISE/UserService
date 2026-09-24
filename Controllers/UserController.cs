using Microsoft.AspNetCore.Mvc;
using Models;
using UserService.Repositories;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;

    public UserController(
        ILogger<UserController> logger,
        IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    // GET: Hent alle brugere
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();

        return Ok(users);
    }

    // GET: Hent én bestemt bruger
    [HttpGet("{userId}", Name = "GetUserById")]
    public async Task<ActionResult<User>> Get(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    // POST: Opret en ny bruger
    [HttpPost]
    public async Task<ActionResult<User>> Create(User user)
    {
        await _userRepository.CreateAsync(user);

        return CreatedAtRoute(
            "GetUserById",
            new { userId = user.Id },
            user
        );
    }

    // PUT: Opdater en bruger
    [HttpPut("{userId}")]
    public async Task<IActionResult> Update(int userId, User user)
    {
        var existingUser = await _userRepository.GetByIdAsync(userId);

        if (existingUser == null)
        {
            return NotFound();
        }

        user.Id = userId;

        await _userRepository.UpdateAsync(userId, user);

        return NoContent();
    }

    // DELETE: Slet en bruger
    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(int userId)
    {
        var existingUser = await _userRepository.GetByIdAsync(userId);

        if (existingUser == null)
        {
            return NotFound();
        }

        await _userRepository.DeleteAsync(userId);

        return NoContent();
    }
}