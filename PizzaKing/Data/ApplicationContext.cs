using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PizzaKing.Models;
using PizzaKing.Models.Checkout;
using PizzaKing.Models.Custom;

public class ApplicationContext : IdentityDbContext<User>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {

        Database.EnsureCreated();
    }

    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ShopCartItem> ShopCartItems { get; set; } = null!;   
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderDetails> OrderDetails { get; set; } = null!;
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<ShopCartItemIngredient> ShopCartItemIngredients { get; set; }
    public DbSet<Review> Reviews { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShopCartItem>()
          .Property(u => u.CreatedAt)
          .HasDefaultValueSql("GETDATE()");

        base.OnModelCreating(modelBuilder);
    }

}