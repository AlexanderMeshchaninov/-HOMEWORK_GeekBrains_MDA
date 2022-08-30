using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingCancelRequestFaultConsumer : IConsumer<Fault<IBookingCancellationEvent>>
{
    public Task Consume(ConsumeContext<Fault<IBookingCancellationEvent>> context)
    {
        Console.WriteLine($"[!] Произошла ошибка при отмене столика для клиента {context.Message.Message.OrderId} [!]");
        //Тут можно обрабатывать ошибки...
        return Task.CompletedTask;
    }
}