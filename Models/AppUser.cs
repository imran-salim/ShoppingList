using Microsoft.AspNetCore.Identity;

namespace ShoppingList.Models;

public class AppUser : IdentityUser<int>
{
    public string? DisplayName { get; set; }

    public ShoppingList? ShoppingList { get; set; }
}
