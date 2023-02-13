using Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Models;

public class SignOperation:BaseEntity
{
    public Guid DocumentId { get; set; }
    
    public Document Document { get; set; }
    
    public string RecipientFirstName { get; set; }
    
    public string RecipientLastName { get; set; }
    
    public string? RecipientPatronymic { get; set; }
    
    public string RecipientIdentificationNumber { get; set; }

    public string RecipientPhone { get; set; }
    
    public ICollection<ConfirmationCode> ConfirmationCodes { get; set; }
    
    public DocumentSigningStatus Status { get; set; }
    
    public string DocumentAccessToken { get; set; }

}

public class SignOperationConfiguration : IEntityTypeConfiguration<SignOperation>
{
    public void Configure(EntityTypeBuilder<SignOperation> item)
    {
        item.HasOne(r => r.Document);
        item.Property(r => r.RecipientFirstName).IsRequired();
        item.Property(r => r.RecipientLastName).IsRequired();
        item.Property(r => r.RecipientIdentificationNumber).IsRequired();

        item.Property(e => e.Status)
            .HasConversion(new EnumToStringConverter<DocumentSigningStatus>());
    }
}