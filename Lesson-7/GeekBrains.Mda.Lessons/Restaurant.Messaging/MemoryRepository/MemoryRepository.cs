using System.Collections.Concurrent;
using Mda.Lessons.Core.IMemoryRepository;
using Timer = System.Timers.Timer;

namespace Restaurant.Messaging.MemoryRepository;

public class MemoryRepository<TEntity> : IMemoryRepository<TEntity> where TEntity : class
{
    private ConcurrentDictionary<Guid, TEntity> _repository = new ConcurrentDictionary<Guid, TEntity>();
    private Timer _timer;
    /// <summary>
    /// Добавляет в репозиторий (в памяти) входящий объект
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="entity"></param>
    public async Task AddOrUpdateAsync(Guid orderId, TEntity entity)
    {
        bool isAdded = _repository.TryAdd(orderId, entity);
        
        if (isAdded)
        {
            _timer = new(30_000);
            _timer.Elapsed += async (sender, e) => await ClearObjectAsync(entity);
            _timer.AutoReset = true;
            _timer.Start();
            
            await Task.CompletedTask;
        }
    }

    /// <summary>
    /// Получаем все объекты содержащиеся в репозитории (ConcurrentDictionary)
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TEntity> Get()
    {
        return _repository.Values;
    }

    private async Task ClearObjectAsync(TEntity? entity)
    {
        var itemToClear = _repository.FirstOrDefault(x => x.Value == entity);
        bool isOk = _repository.TryRemove(itemToClear.Key, out _);

        if (isOk)
        {
            Console.WriteLine($"[!!!] Object with id: {itemToClear}");
            Console.WriteLine($"[!!!] Try to remove method: {itemToClear.Value}");
            Console.WriteLine($"[!!!] Success deleted: {isOk}");

            await Task.CompletedTask;
        }
        else
        {
            await Task.CompletedTask;
        }
    }
}