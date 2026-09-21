using BestStoreMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace BestStoreMVC.Services;

public class DataBaseInitializer
{
    public static async Task SeedDataAsync
        (UserManager<ApplicationUser>? userManager, RoleManager<IdentityRole>? roleManager)
    {
        if(userManager == null || roleManager == null)
        {
            Console.WriteLine("userManager or roleManager is null => exit");
            return;
        }


        var exist = await roleManager.RoleExistsAsync("admin");
        if (!exist) {
            Console.WriteLine("Admin role is not defined and will be created");
            await roleManager.CreateAsync(new IdentityRole("admin"));
        }

        exist = await roleManager.RoleExistsAsync("seller");
        if (!exist)
        {
            Console.WriteLine("Seller role is not defined and will be created");
            await roleManager.CreateAsync(new IdentityRole("seller"));
        }

        exist = await roleManager.RoleExistsAsync("client");
        if (!exist)
        {
            Console.WriteLine("Client role is not defined and will be created");
            await roleManager.CreateAsync(new IdentityRole("client"));
        }


        // check if we have at least one admin user or not
        var adminUsers = await userManager.GetUsersInRoleAsync("admin");
        if (adminUsers.Any())
        {
            // Admin user already exists => exit
            Console.WriteLine("Admin user already exists => exit");
            return;
        }

        ApplicationUser user = new ApplicationUser {

            FirstName = "Admin",
            LastName = "Admin",
            UserName = "admin@admin.com",
            Email = "admin@admin.com",
            Address = "Admin Address",   // أي قيمة
            CreatedAt = DateTime.Now
        };
        var initialPassword = "admin123";
        var result = await userManager.CreateAsync(user, initialPassword);

        if (result.Succeeded)
        {
            // set the user role
            await userManager.AddToRoleAsync(user, "admin");
            Console.WriteLine("Admin user created successfully! Please update the initial password!");
            Console.WriteLine("Email: " + user.Email);
            Console.WriteLine("Initial password: " + initialPassword);
        }

    }
    
}
