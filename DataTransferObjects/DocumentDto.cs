namespace DataTransferObjects;

public class DocumentDto
{
    public Guid Id { get; set; }
    
    public Guid AuthorCompanyId { get; set; }

    public CompanyDto AuthorCompany { get; set; }
    
    public string FileName { get; set; }
    
    public string Path { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
   public SignOperationDto SignOperation { get; set; }
}