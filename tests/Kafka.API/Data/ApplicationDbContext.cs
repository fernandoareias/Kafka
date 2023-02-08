using Kafka.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Kafka.API.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
}