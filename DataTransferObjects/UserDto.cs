using Common.Attributes;

namespace DataTransferObjects;

public class UserDto
{
    [PatchRestricted]
    public string Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Patronymic { get; set; }
    
    [PatchRestricted]
    public string IndividualIndetificationNumber { get; set; }
    
    [PatchRestricted]
    public Guid? CompanyId { get; set; }

    [PatchRestricted]
    public CompanyDto Company { get; set; }
    
    public string PhoneNumber { get; set; }

    public string FullName => $"{LastName} {FirstName} {Patronymic}";
}