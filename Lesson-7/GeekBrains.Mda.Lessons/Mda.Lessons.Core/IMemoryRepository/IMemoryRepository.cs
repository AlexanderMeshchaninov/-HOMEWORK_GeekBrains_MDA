namespace Mda.Lessons.Core.IMemoryRepository;

public interface IMemoryRepository<TEntity> where TEntity : class
{
    Task AddOrUpdateAsync(Guid orderId, TEntity entity);
    IEnumerable<TEntity> Get();
}