using BestStoreMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC.Services;

public class ApplicationDBContext(DbContextOptions options) :IdentityDbContext<ApplicationUser>(options)
{

    public DbSet<Product> Products { get; set; }
}
