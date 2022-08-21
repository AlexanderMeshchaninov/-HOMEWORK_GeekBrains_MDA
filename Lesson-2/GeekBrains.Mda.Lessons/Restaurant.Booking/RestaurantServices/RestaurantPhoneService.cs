using Mda.Lessons.Core.RestaurantServices;
using Restaurant.Booking.Restaurant;

namespace Restaurant.Booking.RestaurantServices;

public sealed class RestaurantPhoneService : IPhoneService<Table>
{
    private readonly List<Table> _tables;
    private Mutex _mutexObj = new Mutex();
    
    /// <summary>
    /// Класс отвечающий за бронирование через Телефон
    /// </summary>
    /// <param name="tablesInfo">Информация о всех столах в ресторане</param>
    public RestaurantPhoneService(List<Table> tablesInfo)
    {
        _tables = tablesInfo;
    }
    
    /// <summary>
    /// Резервирование столика через Телефон
    /// </summary>
    /// <param name="bookedTable">Количество персон</param>
    public Table BookFreeTableByPhone(int countOfPersons)
    {
        _mutexObj.WaitOne();
        
        try
        {
            Console.WriteLine("Добрый день! Подождите секунду я подберу столик и подтвержу вашу бронь, оставайтесь на линии...");

            Table table = _tables.FirstOrDefault(x => x.SeatsCount >= countOfPersons && x.State == State.Free);
            
            Thread.Sleep(5000);

            table?.SetState(State.Booked);
            
            return table;
        }
        finally
        {
            _mutexObj.ReleaseMutex();
        }
    }
    
    /// <summary>
    /// Снятие столика с брони через Телефон
    /// </summary>
    /// <param name="bookedTable">Номер столика</param>
    public Table CancelBookingByPhone(int tableId)
    {
        _mutexObj.WaitOne();

        try
        {
            Console.WriteLine("Добрый день! Введите номер заказанного столика, чтобы снять бронь.");

            Table table = _tables.FirstOrDefault(x => x.Id.Equals(tableId));
        
            Console.WriteLine("Подождите секундочку...");
            Thread.Sleep(5000);
        
            if (table?.State == State.Free)
            {
                Console.WriteLine("Столик уже свободен!");
                return null;
            }
        
            table?.SetState(State.Free);
            
            return table;
        }
        finally
        {
            _mutexObj.ReleaseMutex();
        }
    }
}