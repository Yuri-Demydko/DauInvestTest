using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Models;

public class User:IdentityUser
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string? Patronymic { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime RefreshTokenExpiryTime { get; set; }
    
    public string IndividualIndetificationNumber { get; set; }
    
    public Guid? CompanyId { get; set; }
    
    public Company? Company { get; set; }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> item)
    {
        item.Property(r => r.IndividualIndetificationNumber).IsRequired().HasMaxLength(12);
        item.Property(r => r.FirstName).IsRequired();
        item.Property(r => r.LastName).IsRequired();
        item.Property(r => r.PhoneNumber).IsRequired();
        
        item.HasOne<Company>(r => r.Company)
            .WithMany(r => r.Members)
            .HasForeignKey(r => r.CompanyId);
    }
}