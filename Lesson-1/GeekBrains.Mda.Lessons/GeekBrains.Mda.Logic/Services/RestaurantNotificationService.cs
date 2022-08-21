using GeekBrains.Mda.Logic.Restaurant;

namespace GeekBrains.Mda.Logic.Services;

public sealed class RestaurantNotificationService
{
    private Mutex _mutexObj = new Mutex();
    
    public async Task NotifyBySmsAsync(Table bookedTable)
    {
        Task.Run(async () =>
        {
            await Task.Delay(5000);

            Console.WriteLine(bookedTable is null
                ? "УВЕДОМЛЕНИЕ: К сожалению все столики заняты..."
                : $"УВЕДОМЛЕНИЕ: Готово! Ваш столик номер: {bookedTable.Id}");

        });
    }
    
    public async Task NotifyCancelBySmsAsync(Table canceledTable)
    {
        Task.Run(async () =>
        {
            await Task.Delay(5000);

            Console.WriteLine($"УВЕДОМЛЕНИЕ: Столик номер {canceledTable?.Id}, теперь свободен");
        });
    }
    
    public void NotifyByPhone(Table bookedTable)
    {
        _mutexObj.WaitOne();

        try
        {
            Thread.Sleep(5000);

            Console.WriteLine(bookedTable is null
                ? "ЗВОНОК: К сожалению все столики заняты..."
                : $"ЗВОНОК: Готово! Ваш столик номер: {bookedTable.Id}");
        }
        finally
        {
            _mutexObj.ReleaseMutex();
        }
    }
    
    public void NotifyCancelByPhone(Table canceledTable)
    {
        _mutexObj.WaitOne();

        try
        {
            Thread.Sleep(5000);
            
            Console.WriteLine($"ЗВОНОК: Столик номер {canceledTable?.Id}, теперь свободен");
        }
        finally
        {
            _mutexObj.ReleaseMutex();
        }
    }
}