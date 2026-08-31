using BestStoreMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC.Services;

public class ApplicationDBContext(DbContextOptions options) : DbContext(options)
{

    public DbSet<Product> Products { get; set; }
}
