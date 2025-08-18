using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Models;

namespace ShoppingList.Data;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Models.ShoppingList> ShoppingLists => Set<Models.ShoppingList>();

    public DbSet<FoodItem> FoodItems => Set<FoodItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.ShoppingList>()
            .HasIndex(sl => sl.UserId)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.ShoppingList)
            .WithOne(sl => sl.User)
            .HasForeignKey<Models.ShoppingList>(sl => sl.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Models.ShoppingList>()
            .HasMany(sl => sl.FoodItems)
            .WithOne(fi => fi.ShoppingList)
            .HasForeignKey(fi => fi.ShoppingListId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

