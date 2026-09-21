using System.ComponentModel.DataAnnotations;

namespace BestStoreMVC.Models.DTOs;

public class RegisterDTO
{
    [Required(ErrorMessage = "The First Name field is required"), MaxLength(100)]
    public string FirstName { get; set; } = default!;

    [Required(ErrorMessage = "The Last Name field is required"), MaxLength(100)]
    public string LastName { get; set; } = default!;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = default!;

    [Phone(ErrorMessage = "The format of the Phone Number is not valid"), MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required, MaxLength(200)]
    public string Address { get; set; } = default!;

    [Required, MaxLength(100)]
    public string Password { get; set; } = default!;

    [Required(ErrorMessage = "The Confirm Password field is required")]
    [Compare("Password", ErrorMessage = "Confirm Password and Password do not match")]
    public string ConfirmPassword { get; set; } = default!;
}



