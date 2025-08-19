using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Models;

public class FoodItem
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Range(1, 1000)]
    public int Quantity { get; set; }

    [Required]
    public int ShoppingListId { get; set; }
    
    public ShoppingList ShoppingList { get; set; } = null!;
}
