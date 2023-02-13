namespace DauInvestTest.WebApp.Models.ResponseModels;

public class TokenResponse
{
    public string? AccessToken { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime AccessExpiryTime { get; set; }
    
    public DateTime RefreshExpiryTime { get; set; }
}