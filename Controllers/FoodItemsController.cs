using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Data;
using ShoppingList.Models;

namespace ShoppingList.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FoodItemsController : ControllerBase
{
    private readonly AppDbContext _db;

    public FoodItemsController(AppDbContext db) => _db = db;

    private async Task<int?> GetMyListIdAsync(CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var listId = await _db.ShoppingLists
            .Where(sl => sl.UserId == userId)
            .Select(sl => (int?)sl.Id)
            .SingleOrDefaultAsync(cancellationToken);
        return listId;
    }

    public record UpsertFoodItemRequest(
        [property: Required, StringLength(100)] string Name,
        [property: Range(1, 1000)] int Quantity
    );

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var listId = await GetMyListIdAsync(ct);
        if (listId is null) return NotFound("Shopping list not found.");

        var items = await _db.FoodItems
            .AsNoTracking()
            .Where(fi => fi.ShoppingListId == listId)
            .OrderBy(fi => fi.Name)
            .ToListAsync(ct);

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertFoodItemRequest req, CancellationToken cancellationToken)
    {
        var listId = await GetMyListIdAsync(cancellationToken);
        if (listId is null) return NotFound("Shopping list not found.");

        var item = new FoodItem
        {
            Name = req.Name.Trim(),
            Quantity = req.Quantity,
            ShoppingListId = listId.Value
        };

        _db.FoodItems.Add(item);
        await _db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var listId = await GetMyListIdAsync(cancellationToken);
        if (listId is null) return NotFound("Shopping list not found.");

        var item = await _db.FoodItems
            .AsNoTracking()
            .SingleOrDefaultAsync(fi => fi.Id == id && fi.ShoppingListId == listId, cancellationToken);

        return item is null ? NotFound() : Ok(item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertFoodItemRequest req, CancellationToken cancellationToken)
    {
        var listId = await GetMyListIdAsync(cancellationToken);
        if (listId is null) return NotFound("Shopping list not found.");

        var item = await _db.FoodItems
            .SingleOrDefaultAsync(fi => fi.Id == id && fi.ShoppingListId == listId, cancellationToken);

        if (item is null) return NotFound();

        item.Name = req.Name.Trim();
        item.Quantity = req.Quantity;

        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var listId = await GetMyListIdAsync(cancellationToken);
        if (listId is null) return NotFound("Shopping list not found.");

        var item = await _db.FoodItems
            .SingleOrDefaultAsync(fi => fi.Id == id && fi.ShoppingListId == listId, cancellationToken);

        if (item is null) return NotFound();

        _db.FoodItems.Remove(item);
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
