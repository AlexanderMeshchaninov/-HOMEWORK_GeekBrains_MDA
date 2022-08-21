using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingRequestFaultConsumer : IConsumer<Fault<IBookingRequestEvent>>
{
    public Task Consume(ConsumeContext<Fault<IBookingRequestEvent>> context)
    {
        Console.WriteLine($"[!][!][!] [OrderId {context.Message.Message.OrderId}] Отмена в зале по техническим причинам [!][!][!]");
        //Тут можно обрабатывать ошибки...
        return Task.CompletedTask;
    }
}