namespace DauInvestTest.WebApp.Models.RequestModels;

public class RefreshTokenRequest
{
    public string? AccessToken { get; set; }
    
    public string? RefreshToken { get; set; }
}