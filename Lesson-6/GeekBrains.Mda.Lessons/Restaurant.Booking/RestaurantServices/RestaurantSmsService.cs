using Mda.Lessons.Core.ApplicationSettings;
using Restaurant.Booking.Restaurant;
using State = Mda.Lessons.Core.Restaurant.State;

namespace Restaurant.Booking.RestaurantServices;

public class RestaurantSmsService
{
    private readonly List<Table?> _tables;
    private readonly RestaurantBookingStatus _bookingStatus;
    
    public RestaurantSmsService(List<Table?> tables, RestaurantBookingStatus bookingStatus)
    {
        _tables = tables;
        _bookingStatus = bookingStatus;
    }
    
    /// <summary>
    /// Резервирование столика через СМС
    /// </summary>
    /// <param name="countOfPersons">Количество персон</param>
    public async Task<Table?> BookFreeTableBySmsAsync(int countOfPersons)
    {
        Console.WriteLine(_bookingStatus.BookTableSmsGreeting);

        Table? bookedTable = await Task.Run(() =>
        {
            Table? table = _tables.FirstOrDefault(x => x.SeatsCount >= countOfPersons && x.State == State.Free);

            if (table is not null)
            {
                table?.SetState(State.Booked);
            }
            
            return table;
        });

        return bookedTable;
    }
    
    /// <summary>
    /// Снятие столика с брони через СМС
    /// </summary>
    /// <param name="tableNumber">Номер столика</param>
    public async Task<Table?> CancelBookingBySmsAsync(int tableNumber)
    {
        Console.WriteLine(_bookingStatus.CancelTableGreeting);

        Table? canceledTable = await Task.Run(async () =>
        {
            Table? table = _tables.FirstOrDefault(x => x.TableNumber.Equals(tableNumber));

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

    /// <summary>
    /// Резервирование столика через СМС с присвоением бронируемому объекту номера заказа (для использования совместно с методом отмены по orderId)
    /// </summary>
    /// <param name="orderId">Номер брони</param>
    /// <param name="countOfPersons">Количество персон</param>
    /// <returns></returns>
    public async Task<Table?> BookTableBySmsFromRequestAsync(Guid orderId, int countOfPersons)
    {
        Console.WriteLine(_bookingStatus.BookTableSmsGreeting);

        Table? bookedTable = await Task.Run(() =>
        {
            Table? table = _tables.FirstOrDefault(x => x.SeatsCount >= countOfPersons && x.State == State.Free);

            if (table is not null)
            {
                table.SetState(State.Booked);
                table.OrderId = orderId;
            }
            
            return table;
        });

        return bookedTable;
    }
    
    /// <summary>
    /// Снятие столика с брони через СМС по номеру бронирования (используется в consumer где отмена происходит по причине ошибки CancelInner...)
    /// Дело в том, что в этой ситуации нет возможности получать номер столика, как при "нормальной отмене"
    /// </summary>
    /// <param name="orderId">Номер брони</param>
    /// <returns></returns>
    public async Task<Table?> CancelTableBySmsFromRequestAsync(Guid orderId)
    {
        Console.WriteLine(_bookingStatus.CancelTableGreeting);

        Table? canceledTable = await Task.Run(async () =>
        {
            Table? table = _tables.FirstOrDefault(x => x.OrderId.Equals(orderId));

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