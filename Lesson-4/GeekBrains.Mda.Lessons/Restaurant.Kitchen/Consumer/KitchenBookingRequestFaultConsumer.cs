using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenBookingRequestFaultConsumer : IConsumer<Fault<IBookingRequestEvent>>
{
    public Task Consume(ConsumeContext<Fault<IBookingRequestEvent>> context)
    {
        Console.WriteLine($"[!][!][!] [OrderId {context.Message.Message.OrderId}] Отмена на кухне по техническим причинам [!][!][!]");
        //Тут можно обрабатывать ошибки...
        return Task.CompletedTask;
    }
}