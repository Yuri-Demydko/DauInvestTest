namespace DataTransferObjects;

public class DocumentAccessTokenDto
{
    public Guid Id { get; set; }
    
    public string Token { get; set; }
    
    public Guid DocumentId { get; set; }
    
    public bool Expired { get; set; }
    
}