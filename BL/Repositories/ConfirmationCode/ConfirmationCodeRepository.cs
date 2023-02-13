using AutoMapper;
using BL.Configuration;
using Common.Interfaces;
using DAL;
using DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BL.Repositories.ConfirmationCode;

public class ConfirmationCodeRepository:IRepository<ConfirmationCodeDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    
    private DbSet<DAL.Models.ConfirmationCode> _dbSet => 
        _context.Set<DAL.Models.ConfirmationCode>();

    public ConfirmationCodeRepository(IConfiguration configuration)
    {
        
        _configuration = configuration;
        _mapper = AutomapperConfiguration.GetMapperConfiguration().CreateMapper();
        
        _context = new AppDbContext(configuration,_mapper);
    }
    public IQueryable<ConfirmationCodeDto> Get()
    {
        return _mapper.ProjectTo<ConfirmationCodeDto>(_dbSet)
            .AsQueryable();
    }

    public async Task<ConfirmationCodeDto> CreateAsync(ConfirmationCodeDto item)
    {
        var entity = _mapper.Map<DAL.Models.ConfirmationCode>(item);
        var entityEntry = await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return _mapper.Map<ConfirmationCodeDto>(entityEntry.Entity);
    }

    public async Task<ConfirmationCodeDto> UpdateAsync(ConfirmationCodeDto item, bool force = false)
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