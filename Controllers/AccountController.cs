using BestStoreMVC.Models;
using BestStoreMVC.Models.DTOs;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BestStoreMVC.Controllers;

public class AccountController
    (SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager, EmailSender emailSender) : Controller
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
    [Authorize]
    public async Task<IActionResult> Profile() {

        var user = await userManager.GetUserAsync(User);
        if(user is null) {

            return RedirectToAction("Index", "Home");

        }

        ProfileDTO profileDTO = new ProfileDTO
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,

        };


        return View(profileDTO);
    }
    [HttpPost]
    public async Task<IActionResult> Profile(ProfileDTO profileDTO) {

        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = "Please fill all the required fields with valid values";
            return View(profileDTO);
        }
        var user = await userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToAction("Index", "Home");
        }

        user.FirstName = profileDTO.FirstName;
        user.LastName = profileDTO.LastName;
        user.Email = profileDTO.Email;
        user.UserName = profileDTO.Email;
        user.PhoneNumber = profileDTO.PhoneNumber;
        user.Address = profileDTO.Address;

        var result  =  await userManager.UpdateAsync(user);

        if (result.Succeeded) {

            ViewBag.SuccessMessage = "Profile updated successfully";
        }
        else
        {
            ViewBag.ErrorMessage = "Unable to update the profile: " + result.Errors.First().Description;
        }
            return View(profileDTO);
    }


    public IActionResult ForgetPassword() {

        if (signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Home");
        }
        return View();

    }

    [HttpPost]
    public async Task<IActionResult> ForgetPassword([Required,EmailAddress] string email)
    {
        if(signInManager.IsSignedIn(User))
        {
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.ErrorMessage = ModelState["email"]?.Errors.First().ErrorMessage ?? "Invalid Email ";
            return View();
        }
        ViewBag.Email = email;
        var user =await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            //generate token
            var token  = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetURL = Url.ActionLink("ResetPasswoed", "Account", new { token } ) ?? "Url Error";


            //send email
            var username = user.UserName +" "+user.LastName;
            await emailSender.SendEmailAsync(email,username,"Rest Password" ,resetURL);

        }
        ViewBag.SuccessMessage = "Please check your Email account and click on the Password Reset link!";

        return View();

    }
    public IActionResult AccessDenied() => RedirectToAction("Index", "Home");
}
