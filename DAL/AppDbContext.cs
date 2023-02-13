using System.Reflection;
using AutoMapper;
using Common.Attributes;
using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL;

public sealed class AppDbContext:IdentityDbContext<User>
{
    private const string DbModelsNamespace = "DAL.Models";
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AppDbContext()
    {
        
    }

    public AppDbContext(IConfiguration configuration, IMapper mapper)
    {
        _configuration = configuration;
        _mapper = mapper;
    }

    public AppDbContext(DbContextOptions options,IConfiguration configuration, IMapper mapper) : base(options)
    {
        _configuration = configuration;
        _mapper = mapper;

        Database.Migrate();
    }
    
    public DbSet<Company> Companies { get; set; }
    public DbSet<Document> Documents { get; set; }
    
     public DbSet<SignOperation> SignOperations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new CompanyConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
         builder.ApplyConfiguration(new SignOperationConfiguration());
        builder.ApplyConfiguration(new DocumentConfiguration());
        builder.ApplyConfiguration(new ConfirmationCodeConfiguration());
    }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseNpgsql((_configuration.GetConnectionString("Default"))??"host=localhost;port=5432;database=dauinvest-db;username=postgres;password=postgres;Pooling=true"); 
    }
    
    public async Task<TDto?> PatchAsync<TDto>(TDto? item, object key,bool force=false) where TDto:class
    {
        if (item == null)
            return null;
        
        var dtoType = GetEntityType<TDto>(DbModelsNamespace);
        
        if (dtoType == null)
            return null;
        
        var exist = await FindAsync(dtoType,key);

        if (exist == null) return null;

        var existDto = _mapper.Map<TDto>(exist);
        var properties = existDto.GetType().GetProperties();
        
        item.GetType().GetProperties().ToList().ForEach(r =>
        {
            if ((r.GetCustomAttribute<PatchRestrictedAttribute>() != null && !force)||r.GetValue(item)==null)
            {
                r.SetValue(item,properties.First(p=>p.Name==r.Name).GetValue(existDto));
            }
        });
        Entry(exist).CurrentValues.SetValues(item);
        var result = await SaveChangesAsync();
        return result != 0 ? null : _mapper.Map<TDto>(exist);
    }

    //@TODO: Extract to separate service
    private static Type? GetEntityType<TDto>(string modelsNamespace) where TDto : class 
        => Type.GetType($"{modelsNamespace}.{typeof(TDto).Name.Replace("Dto", "")}");
}