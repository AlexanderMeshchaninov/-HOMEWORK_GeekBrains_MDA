using Mda.Lessons.Core.ApplicationSettings;
using Restaurant.Booking.Restaurant;
using State = Mda.Lessons.Core.Restaurant.State;

namespace Restaurant.Booking.RestaurantServices;

public class RestaurantPhoneService
{
    private readonly List<Table> _tables;
    private readonly RestaurantBookingStatus _bookingStatus;
    private Mutex _mutexObj = new Mutex();
    
    public RestaurantPhoneService(List<Table> tablesInfo, RestaurantBookingStatus bookingStatus)
    {
        _tables = tablesInfo;
        _bookingStatus = bookingStatus;
    }
    
    /// <summary>
    /// Резервирование столика через Телефон
    /// </summary>
    /// <param name="countOfPersons">Количество персон</param>
    public Table BookFreeTableByPhone(int countOfPersons)
    {
        _mutexObj.WaitOne();
        
        try
        {
            Console.WriteLine(_bookingStatus.BookTablePhoneGreeting);

            Table table = _tables.FirstOrDefault(x => x.SeatsCount >= countOfPersons && x.State == State.Free);
            
            Thread.Sleep(5000);

            table?.SetState(State.Booked);
            table.OrderId = Guid.NewGuid();
            table.ClientId = Guid.NewGuid();
            
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
    /// <param name="tableNumber">Номер столика</param>
    public Table CancelBookingByPhone(int tableNumber)
    {
        _mutexObj.WaitOne();

        try
        {
            Console.WriteLine(_bookingStatus.CancelTableGreeting);

            Table table = _tables.FirstOrDefault(x => x.TableNumber.Equals(tableNumber));
        
            Console.WriteLine("Подождите секундочку...");
            Thread.Sleep(5000);
        
            if (table?.State == State.Free)
            {
                Console.WriteLine(_bookingStatus.AlreadyExists);
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