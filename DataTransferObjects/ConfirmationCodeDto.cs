namespace DataTransferObjects;

public class ConfirmationCodeDto
{
    public Guid Id { get; set; }
        
    public Guid SignOperationId { get; set; }
        
    public SignOperationDto SignOperation { get; set; }
        
    public string Code { get; set; }
        
    public bool IsExpired { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
}