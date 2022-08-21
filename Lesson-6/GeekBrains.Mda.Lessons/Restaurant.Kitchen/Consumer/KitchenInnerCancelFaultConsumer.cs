using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenInnerCancelFaultConsumer : IConsumer<Fault<IBookingInnerCancelEvent>>
{
    /// <summary>
    /// Подписка на отмену на кухне при возникновении ЧП, срабатывает автоматически если при этом возникла ошибка
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task Consume(ConsumeContext<Fault<IBookingInnerCancelEvent>> context)
    {
        Console.WriteLine($"[!] Произошла ошибка при отмене кухни для клиента {context.Message.Message.OrderId} [!]");
        //Тут можно обрабатывать ошибки...
        return Task.CompletedTask;
    }
}