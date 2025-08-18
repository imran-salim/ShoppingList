using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Models;

public class FoodItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [Required]
    [Range(1, 1000)]
    public int Quantity { get; set; }

    [Required]
    public int ListId { get; set; }
}
