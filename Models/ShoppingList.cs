using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Models;

public class ShoppingList
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
}
