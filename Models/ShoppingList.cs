using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Models;

public class ShoppingList
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public required User User { get; set; }

    [Required]
    public List<FoodItem> FoodItems { get; set; } = new();
}
