using Microsoft.AspNetCore.Mvc;
using Models;
using UserService.Repositories;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using UserService.RabbitMQ;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly RabbitMQService _rabbitMqService;


    public UserController(
        ILogger<UserController> logger,
        RabbitMQService rabbitMqService,
        IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _rabbitMqService = rabbitMqService;

        var hostName = System.Net.Dns.GetHostName();
        var ips = System.Net.Dns.GetHostAddresses(hostName);
        var _ipaddr = ips.First().MapToIPv4().ToString();
        _logger.LogInformation(1, $"XYZ Service responding from {_ipaddr}");
    }

    [HttpGet("version")]
    public async Task<Dictionary<string, string>> GetVersion()
    {
        var properties = new Dictionary<string, string>();
        var assembly = typeof(Program).Assembly;
        properties.Add("service", "HaaV User Service"); // eller "HaaV Catalog Service
        var ver = FileVersionInfo.GetVersionInfo(typeof(Program)
            .Assembly.Location).ProductVersion;
        properties.Add("version", ver!);
        try
        {
            var hostName = System.Net.Dns.GetHostName();
            var ips = await System.Net.Dns.GetHostAddressesAsync(hostName);
            var ipa = ips.First().MapToIPv4().ToString();
            properties.Add("hosted-at-address", ipa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            properties.Add("hosted-at-address", "Could not resolve IP-address");
        }

        return properties;
    }

    //[Authorize(Roles = "User")]
    [HttpGet("GetAll")]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();

        return Ok(new { users, message = "You're authorized" });
    }


    [Authorize(Roles = "User")]
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> Get(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("Create")]
    public async Task<ActionResult<User>> Create([FromBody]User user)
    {
        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, user.Password);

        user.Password = string.Empty;

        await _userRepository.CreateAsync(user);

        return Ok(user);
    }

    [Authorize]
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

    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(int userId)
    {
        var existingUser = await _userRepository.GetByIdAsync(userId);

        if (existingUser == null)
        {
            return NotFound();
        }

        bool deleted = await _userRepository.DeleteAsync(userId);

        if (!deleted)
        {
            return StatusCode(500);
        }
        
        // Send UserDeleted event via RabbitMQ
        await _rabbitMqService.PublishUserDeletedAsync(userId);

        return NoContent();
    }
    
    [HttpGet("username/{username}")]
    public async Task<ActionResult<User>> GetByUsername(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user is null)
            return NotFound();

        return Ok(user);
    }
}