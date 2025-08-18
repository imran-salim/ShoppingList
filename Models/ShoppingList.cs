using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Models;

public class ShoppingList
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public AppUser User { get; set; } = null!;

    public List<FoodItem> FoodItems { get; set; } = [];
}
