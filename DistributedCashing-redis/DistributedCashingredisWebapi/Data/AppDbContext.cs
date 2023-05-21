using Microsoft.EntityFrameworkCore;

namespace DistributedCashingredisWebapi;

public class AppDbContext : DbContext 
{
    public DbSet<Driver> Drivers {set;get;}
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base (options){}
    



}