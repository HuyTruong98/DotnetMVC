using Microsoft.EntityFrameworkCore;
using OnlineStoreMVC.Models;

namespace OnlineStoreMVC.Data
{
  public class OnlineStoreDBContext : DbContext
  {
    public OnlineStoreDBContext(DbContextOptions<OnlineStoreDBContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<Promotion> Promotions { get; set; }
  }
}