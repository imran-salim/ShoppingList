using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Data;
using ShoppingList.Models;

namespace ShoppingList.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly AppDbContext _db;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
    }

    public record RegisterRequest(
        [Required, EmailAddress] string Email,
        [Required, MinLength(6)] string Password,
        string? DisplayName
    );

    public record LoginRequest(
        [Required, EmailAddress] string Email,
        [Required] string Password,
        bool RememberMe = false
    );

    public record AuthResponse(
        string Id,
        string Email,
        string? DisplayName
    );

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(req.Email);
        if (existingUser is not null)
        {
            return Conflict("Email already in use.");
        }

        var user = new AppUser
        {
            Email = req.Email,
            UserName = req.Email,
            DisplayName = req.DisplayName
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        _db.ShoppingLists.Add(new Models.ShoppingList { UserId = user.Id });
        await _db.SaveChangesAsync(cancellationToken);

        await _signInManager.SignInAsync(user, isPersistent: true);

        return Ok(new AuthResponse(user.Id.ToString(), user.Email!, user.DisplayName));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var result = await _signInManager.PasswordSignInAsync(req.Email, req.Password, req.RememberMe, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Unauthorized("Invalid credentials.");
        }
        var user = await _userManager.FindByEmailAsync(req.Email);
        return Ok(new AuthResponse(user!.Id.ToString(), user.Email!, user.DisplayName));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userManager.Users
            .Where(u => u.Id == userId)
            .Select(u => new AuthResponse(u.Id.ToString(), u.Email!, u.DisplayName))
            .FirstOrDefaultAsync(ct);

        return Ok(user);
    }
}
