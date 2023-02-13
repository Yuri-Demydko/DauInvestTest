using AutoMapper;
using BL.Configuration;
using Common.Interfaces;
using DAL;
using DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BL.Repositories.User;

public class UserRepository:IRepository<UserDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    
    private readonly IConfiguration _configuration;

    public UserRepository(IConfiguration configuration)
    {
        
        _configuration = configuration;
        _mapper = AutomapperConfiguration.GetMapperConfiguration().CreateMapper();
        
        _context = new AppDbContext(configuration,_mapper);
    }

    private DbSet<DAL.Models.User> _dbSet => 
        _context.Set<DAL.Models.User>();
    public IQueryable<UserDto> Get()
    {
       return _mapper.ProjectTo<UserDto>(_dbSet.Include(r=>r.Company))
                   .AsQueryable();
    }

    public async Task<UserDto> CreateAsync(UserDto item)
    {
        var user = _mapper.Map<DAL.Models.User>(item);
        var entityEntry = await _dbSet.AddAsync(user);
        await _context.SaveChangesAsync();
        return _mapper.Map<UserDto>(entityEntry.Entity);
    }

    public async Task<UserDto> UpdateAsync(UserDto item,bool force=false)
    {
        return await _context.PatchAsync(item, item.Id,force);
    }
    
    public async Task DeleteAsync(object key)
    {
        if (key is string strKey)
        {
            var entity = new DAL.Models.User(){
                Id = strKey
            };

            var entry = _context.Entry(entity);
            _context.Remove(entry);
            await _context.SaveChangesAsync();
        }
        
    }
}