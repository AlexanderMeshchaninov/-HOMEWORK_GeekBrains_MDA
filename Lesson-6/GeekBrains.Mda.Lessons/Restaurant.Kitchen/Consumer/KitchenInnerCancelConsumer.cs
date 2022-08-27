using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenInnerCancelConsumer : IConsumer<IBookingInnerCancelEvent>
{
    /// <summary>
    /// Подписка на отмену на кухне при возникновении ЧП, срабатывает автоматически
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task Consume(ConsumeContext<IBookingInnerCancelEvent> context)
    {
        Console.WriteLine($"[!] Кухня приостановила заказ клиента {context.Message.ClientId} [!]");

        return Task.CompletedTask;
    }
}