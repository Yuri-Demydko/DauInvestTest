using System.ComponentModel.DataAnnotations;

namespace DauInvestTest.WebApp.Models.RequestModels;

public class DocumentSignRequest
{
    [Required]
    public string RecipientFirstName { get; set; }
    
    [Required]
    public string RecipientLastName { get; set; }
    
    public string? RecipientPatronymic { get; set; }
    
    [Required]
    public string RecipientIdentificationNumber { get; set; }

    [Required]
    public string RecipientPhone { get; set; }
}