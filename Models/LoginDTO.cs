using System.ComponentModel.DataAnnotations;

namespace BestStoreMVC.Models;

    public class LoginDTO
    {

    [Required]
    public string Email { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;

    
    public bool RememberMe { get; set; }     


    }

