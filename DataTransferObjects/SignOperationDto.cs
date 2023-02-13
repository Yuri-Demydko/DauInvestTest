using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Common.Enums;

namespace DataTransferObjects;

public class SignOperationDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    
    public DocumentDto Document { get; set; }
    
    public string RecipientFirstName { get; set; }
    
    public string RecipientLastName { get; set; }
    
    public string? RecipientPatronymic { get; set; }
    
    public string RecipientIdentificationNumber { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DocumentSigningStatus Status { get; set; }
    
    public string DocumentAccessToken { get; set; }
    
    public string RecipientPhone { get; set; }

    public string RecipientData =>
        $"{RecipientLastName} {RecipientFirstName} {(!string.IsNullOrWhiteSpace(RecipientPatronymic) ? RecipientPatronymic + " " : "")} " +
        $"{RecipientPhone} " +
        $"{RecipientIdentificationNumber}";

    public string AuthorCompanyData =>
        Document?.AuthorCompany?.CompanyData;

    public ICollection<ConfirmationCodeDto> ConfirmationCodes { get; set; }
}