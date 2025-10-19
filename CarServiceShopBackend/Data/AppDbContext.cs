using Microsoft.EntityFrameworkCore;
using CarServiceShopBackend.Models;

namespace CarServiceShopBackend.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Car> Cars { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Part> Parts { get; set; }
    
    
    
}