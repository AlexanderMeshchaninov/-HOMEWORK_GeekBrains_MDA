using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantWaitingForAGuestFault : IConsumer<Fault<IBookingApprovedEvent>>
{
    public Task Consume(ConsumeContext<Fault<IBookingApprovedEvent>> context)
    {
        Console.WriteLine($"[!] Произошла ошибка при ожидании клиента {context.Message.Message.OrderId} [!]");
        //Тут можно обрабатывать ошибки...
        return Task.CompletedTask;
    }
}