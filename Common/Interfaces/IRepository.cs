namespace Common.Interfaces;

public interface IRepository<TDto> where TDto:class
{
    public IQueryable<TDto> Get();

    public Task<TDto> CreateAsync(TDto item);
    
    public Task<TDto> UpdateAsync(TDto item,bool force=false);

    public Task DeleteAsync(object key);
}