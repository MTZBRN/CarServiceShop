using Microsoft.EntityFrameworkCore;

namespace CarServiceShopBackend.DbContext;

public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<CarServiceShopBackend.Models.Car> Cars { get; set; }
    public DbSet<CarServiceShopBackend.Models.Service> Services { get; set; }
    public DbSet<CarServiceShopBackend.Models.Part> Parts { get; set; }
    
    
    
}