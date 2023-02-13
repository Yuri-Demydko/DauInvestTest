using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Models;

public class Document:BaseEntity
{
    public Guid AuthorCompanyId { get; set; }

    public Company AuthorCompany { get; set; }
    
    public string FileName { get; set; }
    
    public string Path { get; set; }
    
    public SignOperation SignOperation { get; set; }

    public ICollection<ConfirmationCode> ConfirmationCodes { get; set; }
}
public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> item)
    {
        item.HasOne(r => r.AuthorCompany);
        item.Property(r => r.FileName).IsRequired();
        item.Property(r => r.Path).IsRequired();
        item.HasOne(r => r.SignOperation).WithOne(r => r.Document);
    }
}