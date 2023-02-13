using AutoMapper;
using BL.Configuration;
using Common.Interfaces;
using DAL;
using DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BL.Repositories.Company;

public class CompanyRepository:IRepository<CompanyDto>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private DbSet<DAL.Models.Company> _dbSet => 
        _context.Set<DAL.Models.Company>();

    public CompanyRepository(IConfiguration configuration)
    {
        
        _configuration = configuration;
        _mapper = AutomapperConfiguration.GetMapperConfiguration().CreateMapper();
        
        _context = new AppDbContext(configuration,_mapper);
    }

    public IQueryable<CompanyDto> Get()
    {
        return _mapper.ProjectTo<CompanyDto>(_dbSet.Include(r=>r.Members))
            .AsQueryable();
    }

    public async Task<CompanyDto> CreateAsync(CompanyDto item)
    {
        var company = _mapper.Map<DAL.Models.Company>(item);
        var entityEntry = await _context.Companies.AddAsync(company);
        await _context.SaveChangesAsync();
        return _mapper.Map<CompanyDto>(entityEntry.Entity);
    }

    public async Task<CompanyDto> UpdateAsync(CompanyDto item,bool force=false)
    {
        return await _context.PatchAsync(item, item.Id,force);
    }

    public async Task DeleteAsync(object key)
    {
        if (key is Guid guidKey)
        {
            var entity = new DAL.Models.Company(){
                Id = guidKey
            };

            var entry = _context.Entry(entity);
            _context.Remove(entry);
            await _context.SaveChangesAsync();
        }
        
    }
}
