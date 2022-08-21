using Mda.Lessons.Core.ApplicationSettings;
using Restaurant.Booking.Restaurant;
using State = Mda.Lessons.Core.Restaurant.State;

namespace Restaurant.Booking.RestaurantServices;

public class RestaurantSmsService
{
    private readonly List<Table> _tables;
    private readonly RestaurantBookingStatus _bookingStatus;
    
    public RestaurantSmsService(List<Table> tables, RestaurantBookingStatus bookingStatus)
    {
        _tables = tables;
        _bookingStatus = bookingStatus;
    }
    
    /// <summary>
    /// Резервирование столика через СМС
    /// </summary>
    /// <param name="countOfPersons">Количество персон</param>
    public async Task<Table> BookFreeTableBySmsAsync(int countOfPersons)
    {
        Console.WriteLine(_bookingStatus.BookTableSmsGreeting);

        Table bookedTable = await Task.Run(() =>
        {
            Table table = _tables.FirstOrDefault(x => x.SeatsCount >= countOfPersons && x.State == State.Free);
            
            table?.SetState(State.Booked);
            table.OrderId = Guid.NewGuid();
            table.ClientId = Guid.NewGuid();
            
            return table;
        });

        return bookedTable;
    }
    
    /// <summary>
    /// Снятие столика с брони через СМС
    /// </summary>
    /// <param name="tableNumber">Номер столика</param>
    public async Task<Table> CancelBookingBySmsAsync(int tableNumber)
    {
        Console.WriteLine(_bookingStatus.CancelTableGreeting);

        Table canceledTable = await Task.Run(async () =>
        {
            Table table = _tables.FirstOrDefault(x => x.TableNumber.Equals(tableNumber));

            if (table?.State == State.Free)
            {
                Console.WriteLine(_bookingStatus.AlreadyExists);
                return null;
            }
            
            table?.SetState(State.Free);

            return table;
        });
        
        return canceledTable;
    }
}