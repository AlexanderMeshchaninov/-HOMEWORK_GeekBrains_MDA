using MassTransit;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingRequestFaultConsumer : IConsumer<Fault<IBookingRequestEvent>>
{
    private readonly IBus _bus;

    public RestaurantBookingRequestFaultConsumer(IBus bus)
    {
        _bus = bus;
    }
    
    public async Task Consume(ConsumeContext<Fault<IBookingRequestEvent>> context)
    {
        Console.WriteLine($"[!] ОТМЕНА В ЗАЛЕ для клиента {context.Message.Message.ClientId} по техническим причинам [!]");
        
        await _bus.Publish<ITableCanceledEvent>(new TableCanceledEvent(
            context.Message.Message.OrderId,
            context.Message.Message.ClientId,
            context.Message.Message.PreOrder,
            context.Message.Message.TableNumber,
            0,
            context.Message.Message.CountOfPersons,
            context.Message.Message.Message,
            context.Message.Message.Ready,
            context.Message.Message.CreationDate
        ));
        
        await context.ConsumeCompleted;
    }
}