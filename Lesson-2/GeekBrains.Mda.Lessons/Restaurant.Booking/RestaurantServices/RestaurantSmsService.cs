using Mda.Lessons.Core.RestaurantServices;
using Restaurant.Booking.Restaurant;

namespace Restaurant.Booking.RestaurantServices;

public sealed class RestaurantSmsService : ISmsService<Table>
{
    private readonly List<Table> _tables;
    
    /// <summary>
    /// Класс отвечающий за бронирование через СМС
    /// </summary>
    /// <param name="tablesInfo">Информация о всех столах в ресторане</param>
    public RestaurantSmsService(List<Table> tablesInfo)
    {
        _tables = tablesInfo;
    }

    /// <summary>
    /// Резервирование столика через СМС
    /// </summary>
    /// <param name="bookedTable">Количество персон</param>
    public async Task<Table> BookFreeTableBySmsAsync(int countOfPersons)
    {
        Console.WriteLine("Добрый день! Подождите секунду я подберу столик и подтвержу вашу бронь, вам придет уведомление...");

        Table bookedTable = await Task.Run(() =>
        {
            Table table = _tables.FirstOrDefault(x => x.SeatsCount >= countOfPersons && x.State == State.Free);

            table?.SetState(State.Booked);

            return table;
        });

        return bookedTable;
    }
    
    /// <summary>
    /// Снятие столика с брони через СМС
    /// </summary>
    /// <param name="bookedTable">Номер столика</param>
    public async Task<Table> CancelBookingBySmsAsync(int tableId)
    {
        Console.WriteLine("Добрый день! Введите номер заказанного столика, чтобы снять бронь.");

        Table canceledTable = await Task.Run(async () =>
        {
            Table table = _tables.FirstOrDefault(x => x.Id.Equals(tableId));

            if (table?.State == State.Free)
            {
                Console.WriteLine("Столик уже свободен!");
                return null;
            }
            
            table?.SetState(State.Free);

            return table;

        });
        
        return canceledTable;
    }
}