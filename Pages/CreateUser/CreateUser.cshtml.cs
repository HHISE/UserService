using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UserService.Pages;

public class CreateModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public CreateModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [BindProperty]
    public NewUser NewUser { get; set; } = new();

    public string? Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        using HttpClient client =
            _clientFactory.CreateClient("haavGateway");

        try
        {
            var response = await client.PostAsJsonAsync(
                "User/Create", NewUser);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Brugeren blev oprettet.";
                return RedirectToPage("/Create");
            }

            ModelState.AddModelError(string.Empty,
                "Det lykkedes ikke at oprette brugeren.");

            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
    }
}

public class NewUser
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "User";

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}