using BestStoreMVC.Models;
using BestStoreMVC.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Controllers;

public class AccountController
    (SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) : Controller
{
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDTO registerDTO)
    {

        if (!ModelState.IsValid)
        {
            return View(registerDTO);
        }

        ApplicationUser user = new ApplicationUser
        {
            UserName = registerDTO.Email,
            Email = registerDTO.Email,

            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            Address = registerDTO.Address,
            PhoneNumber = registerDTO.PhoneNumber,
            CreatedAt = DateTime.Now,
        };


        var result = await userManager.CreateAsync(user, registerDTO.Password);

        if (result.Succeeded)
        {

            await userManager.AddToRoleAsync(user, "client");

            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index" , "Home");
        }

        foreach(var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View(registerDTO);
    }


 
    public async Task<IActionResult> Logout(){
    
        if(signInManager.IsSignedIn(User) ){


                await signInManager.SignOutAsync();
        }
            return RedirectToAction("Index" , "Home");



    }



    public IActionResult Login()
    {
        if (signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO loginDTO)
    {
        if (signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Home");
        }
        if (!ModelState.IsValid) {
        
        return View(loginDTO);
        }

        var  result= await signInManager.PasswordSignInAsync(loginDTO.Email,loginDTO.Password,loginDTO.RememberMe,false);
        if(result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {

            ViewBag.ErrorMassage = "PLZ insert a vaild attemp";
        }



        return View(loginDTO);
    }
}
