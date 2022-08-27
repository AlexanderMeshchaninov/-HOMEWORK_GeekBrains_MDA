using MassTransit;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenBookingRequestFaultConsumer : IConsumer<Fault<IBookingRequestEvent>>
{
    /// <summary>
    /// Подписка на событие бронирования столика на кухне если возникла ошибка
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<Fault<IBookingRequestEvent>> context)
    {
        Console.WriteLine($"[!] ОТМЕНА НА КУХНЕ для клиента {context.Message.Message.ClientId} по техническим причинам [!]");
        //Тут можно обрабатывать ошибки...
        await context.ConsumeCompleted;
    }
}