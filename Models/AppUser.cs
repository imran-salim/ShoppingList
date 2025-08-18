using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ShoppingList.Models;

public class AppUser : IdentityUser<int>
{

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    public ShoppingList? ShoppingList { get; set; }
}
