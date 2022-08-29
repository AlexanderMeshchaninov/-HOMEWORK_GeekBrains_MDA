using Mda.Lessons.Core.ApplicationSettings;
using Microsoft.Extensions.Logging;
using Restaurant.Booking.Restaurant;
using State = Mda.Lessons.Core.Restaurant.State;

namespace Restaurant.Booking.RestaurantServices;

public class RestaurantPhoneService
{
    private readonly List<Table?> _tables;
    private readonly RestaurantBookingStatus _bookingStatus;
    private Mutex _mutexObj = new Mutex();

    public RestaurantPhoneService(
        List<Table?> tablesInfo, 
        RestaurantBookingStatus bookingStatus)
    {
        _tables = tablesInfo;
        _bookingStatus = bookingStatus;
    }
    
    /// <summary>
    /// Резервирование столика через Телефон
    /// </summary>
    /// <param name="countOfPersons">Количество персон</param>
    public Table? BookFreeTableByPhone(int countOfPersons)
    {
        _mutexObj.WaitOne();
        
        try
        {
            Console.WriteLine(_bookingStatus.BookTablePhoneGreeting);

            Table? bookedTable = _tables.FirstOrDefault(x => x.SeatsCount >= countOfPersons && x.State == State.Free);
            
            Thread.Sleep(5000);

            if (bookedTable is not null)
            {
                bookedTable?.SetState(State.Booked);
                return bookedTable;
            }

            return bookedTable;
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
    public Table? CancelBookingByPhone(int tableNumber)
    {
        _mutexObj.WaitOne();

        try
        {
            Console.WriteLine(_bookingStatus.CancelTableGreeting);

            Table? canceledTable = _tables.FirstOrDefault(x => x.TableNumber.Equals(tableNumber));
        
            Console.WriteLine("Подождите секундочку...");
            Thread.Sleep(5000);
        
            if (canceledTable?.State == State.Free)
            {
                Console.WriteLine(_bookingStatus.AlreadyExists);
                return null;
            }
            
            if (canceledTable is not null)
            {
                canceledTable?.SetState(State.Free);
                return canceledTable;
            }

            return canceledTable;
        }
        finally
        {
            _mutexObj.ReleaseMutex();
        }
    }
}