using AutoMapper;
using BL.Configuration;
using Common.Interfaces;
using DAL;
using DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BL.Repositories.SignOperation;

public class SignOperationRepository:IRepository<SignOperationDto>
{
    //@TODO REMOVE DEPENDENCIES
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    
    private readonly IConfiguration _configuration;
    
    private DbSet<DAL.Models.SignOperation> _dbSet => 
        _context.Set<DAL.Models.SignOperation>();

    public SignOperationRepository(IConfiguration configuration)
    {
        
        _configuration = configuration;
        _mapper = AutomapperConfiguration.GetMapperConfiguration().CreateMapper();
        _context = new AppDbContext(configuration,_mapper);
    }

    public IQueryable<SignOperationDto> Get()
    {
        return _mapper.ProjectTo<SignOperationDto>(_dbSet
                ,new{},(dto)=>dto.Document,dto=>dto.Document.AuthorCompany,dto=>dto.Document.AuthorCompany.Owner,dto=>dto.ConfirmationCodes)
            .AsQueryable();
    }

    public async Task<SignOperationDto> CreateAsync(SignOperationDto item)
    {
        var entity = _mapper.Map<DAL.Models.SignOperation>(item);
        var entityEntry = await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<SignOperationDto>(entityEntry.Entity);
    }

    public async Task<SignOperationDto> UpdateAsync(SignOperationDto item, bool force = false)
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