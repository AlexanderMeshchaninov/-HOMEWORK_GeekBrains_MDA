using GeekBrains.Mda.Logic.Restaurant;

namespace GeekBrains.Mda.Logic.Services;

public sealed class RestaurantAutoCancelService
{
    public RestaurantAutoCancelService(List<Table> tables, int time)
    {
        Timer timer = new Timer(async x => await Cancel(tables), tables, 0, time);
    }

    private async Task Cancel(List<Table> tables)
    {
        await Task.Run(async () =>
        {
            foreach (var table in tables)
            {
                if (table.State is State.Booked)
                {
                    Console.WriteLine($"Столик номер {table.Id} освобожден! Бронь, к сожалению, слетела!");
                    table.SetState(State.Free);
                }
            }

        });
    }
}