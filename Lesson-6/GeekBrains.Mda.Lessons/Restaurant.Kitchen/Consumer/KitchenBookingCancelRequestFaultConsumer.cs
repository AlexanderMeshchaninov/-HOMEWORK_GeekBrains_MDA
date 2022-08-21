using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenBookingCancelRequestFaultConsumer : IConsumer<Fault<IBookingCancelRequestEvent>>
{
    /// <summary>
    /// Подписка на событие отмены бронирования столика на кухне если возникла ошибка
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <returns></returns>
    public Task Consume(ConsumeContext<Fault<IBookingCancelRequestEvent>> context)
    {
        Console.WriteLine($"[!] Произошла ошибка при отмене кухни для клиента {context.Message.Message.OrderId} [!]");
        //Тут можно обрабатывать ошибки...
        return Task.CompletedTask;
    }
}