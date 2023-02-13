using AutoMapper;
using BL.Configuration;
using Common.Interfaces;
using DAL;
using DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BL.Repositories.Document;

public class DocumentRepository:IRepository<DocumentDto>
{
    //@TODO REMOVE DEPENDENCIES
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    
    private readonly IConfiguration _configuration;
    
    private DbSet<DAL.Models.Document> _dbSet => 
        _context.Set<DAL.Models.Document>();

    public DocumentRepository(IConfiguration configuration)
    {
        
        _configuration = configuration;
        _mapper = AutomapperConfiguration.GetMapperConfiguration().CreateMapper();
        
        _context = new AppDbContext(configuration,_mapper);
    }

    public IQueryable<DocumentDto> Get()
    {
        return _mapper.ProjectTo<DocumentDto>(_dbSet.Include(r=>r.AuthorCompany))
            .AsQueryable();
    }

    public async Task<DocumentDto> CreateAsync(DocumentDto item)
    {
        var document = _mapper.Map<DAL.Models.Document>(item);
        var entityEntry = await _dbSet.AddAsync(document);
        await _context.SaveChangesAsync();
        return _mapper.Map<DocumentDto>(entityEntry.Entity);
    }

    public async Task<DocumentDto> UpdateAsync(DocumentDto item, bool force = false)
    {
        return await _context.PatchAsync(item, item.Id,force);
    }

    public async Task DeleteAsync(object key)
    {
        if (key is string strKey)
        {
            var entity = new DAL.Models.User()
            {
                Id = strKey
            };

            var entry = _context.Entry(entity);
            entry.State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }
    }
}