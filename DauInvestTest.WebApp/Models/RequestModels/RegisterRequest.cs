using System.ComponentModel.DataAnnotations;

namespace DauInvestTest.WebApp.Models.RequestModels;

public class RegisterRequest
{
    [Required(ErrorMessage = "First Name is required")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Last Name is required")]
    public string LastName { get; set; }
    
    public string? Patronymic { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Phone is required")]
    public string PhoneNumber { get; set; }
    
    [Required(ErrorMessage = "IIN is required")]
    public string IndividualIndentificationNumber { get; set; }
}