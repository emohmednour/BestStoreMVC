using BestStoreMVC.Migrations;
using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("Program started");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var cs = builder.Configuration.GetConnectionString("cs");
builder.Services.AddDbContext<ApplicationDBContext>(
    opt=>opt.UseSqlServer(cs));

builder.Services.AddIdentity<ApplicationUser,IdentityRole>(
    option =>
    {
        option.Password.RequiredLength = 6;
        option.Password.RequireNonAlphanumeric = false;
        option.Password.RequireUppercase = false;
        option.Password.RequireLowercase = false;

    }).AddEntityFrameworkStores<ApplicationDBContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var UserManager = scope.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>))
        as UserManager<ApplicationUser>;

    var roleManger  = scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>))
        as RoleManager<IdentityRole>;
    

    await DataBaseInitializer.SeedDataAsync(UserManager, roleManger);
}



    app.Run();
