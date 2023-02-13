using Common.Attributes;

namespace DataTransferObjects;

public class CompanyDto
{
    [PatchRestricted]
    public Guid Id { get; set; }
    
    public string Title { get; set; }
    
    [PatchRestricted]
    public string BusinessIdentificationNumber { get; set; }

    public string ContactPhone { get; set; }
    
    
    public ICollection<UserDto> Members { get; set; }
    
    [PatchRestricted]
    public UserDto Owner { get; set; }

    [PatchRestricted]
    public string? OwnerId { get; set; }
    
    [PatchRestricted]
    public string CompanyData =>
        $"{Title} {BusinessIdentificationNumber} {ContactPhone}";
}