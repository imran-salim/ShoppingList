using System.ComponentModel.DataAnnotations;

namespace ShoppingList.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Username { get; set; }

    [Required]
    [StringLength(100)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required]
    public required ShoppingList ShoppingList { get; set; } = null!;
}
