using ShoppingList.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoppingList.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<FoodItem> FoodItems { get; set; }
    public DbSet<Models.ShoppingList> ShoppingLists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
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

