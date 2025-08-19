using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Data;

namespace ShoppingList.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ShoppingListsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ShoppingListsController(AppDbContext db) => _db = db;

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var list = await _db.ShoppingLists
            .AsNoTracking()
            .Include(sl => sl.FoodItems)
            .SingleOrDefaultAsync(sl => sl.UserId == userId, cancellationToken);

        if (list is null) return NotFound("Shopping list not found.");
        return Ok(list);
    }
}
