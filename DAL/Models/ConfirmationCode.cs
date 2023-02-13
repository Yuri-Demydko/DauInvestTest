using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Models;

public class ConfirmationCode:BaseEntity
{
    public Guid SignOperationId { get; set; }
    
    public SignOperation SignOperation { get; set; }
    
    public string Code { get; set; }
    
    public bool IsExpired { get; set; }
    
}

public class ConfirmationCodeConfiguration : IEntityTypeConfiguration<ConfirmationCode>
{
    public void Configure(EntityTypeBuilder<ConfirmationCode> item)
    {
        item.HasOne(r => r.SignOperation)
            .WithMany(r => r.ConfirmationCodes)
            .HasForeignKey(r => r.SignOperationId);

        item.HasQueryFilter(r => !r.IsExpired);
    }
}