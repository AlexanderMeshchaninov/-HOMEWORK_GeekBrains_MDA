using GeekBrains.Mda.Logic.Restaurant;

namespace GeekBrains.Mda.Logic.Services;

public sealed class RestaurantSmsService
{
    private readonly List<Table> _tables;
    public RestaurantSmsService(List<Table> tablesInfo)
    {
        _tables = tablesInfo;
    }

    public async Task<Table> BookFreeTableBySmsAsync(int countOfPersons)
    {
        Console.WriteLine("Добрый день! Подождите секунду я подберу столик и подтвержу вашу бронь, вам придет уведомление");

        Table bookedTable = await Task.Run(() =>
        {
            Table table = _tables.FirstOrDefault(x => x.SeatsCount >= countOfPersons && x.State == State.Free);

            table?.SetState(State.Booked);

            return table;
        });

        return bookedTable;
    }
    
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
            
            await Task.Delay(5000);

            table?.SetState(State.Free);

            return table;
        });

        return canceledTable;
    }
}