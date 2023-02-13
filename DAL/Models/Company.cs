using System.ComponentModel.DataAnnotations.Schema;
using DelegateDecompiler;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Models;

//Pretty simplified company model, describes only few necessary properties
public class Company:BaseEntity
{
    public string Title { get; set; }
    
    public string BusinessIdentificationNumber { get; set; }

    public string ContactPhone { get; set; }
    
    public ICollection<User> Members { get; set; }

    [NotMapped] [Computed] public User Owner => Members?.FirstOrDefault(r => r.Id == OwnerId);

    public string OwnerId { get; set; }
}
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> item)
    {
        item.Property(r => r.Title).IsRequired();
        item.Property(r => r.ContactPhone).IsRequired();
        item.Property(r => r.BusinessIdentificationNumber).IsRequired().HasMaxLength(12);
       
        item.HasMany(r => r.Members)
            .WithOne(r => r.Company)
            .HasForeignKey(r => r.CompanyId);
    }
}