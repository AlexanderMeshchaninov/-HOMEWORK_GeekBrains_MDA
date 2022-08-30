using MassTransit;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenBookingRequestFaultConsumer : IConsumer<Fault<IBookingRequestEvent>>
{
    private readonly IBus _bus;

    public KitchenBookingRequestFaultConsumer(IBus bus)
    {
        _bus = bus;
    }
    public async Task Consume(ConsumeContext<Fault<IBookingRequestEvent>> context)
    {
        Console.WriteLine($"[!] ОТМЕНА НА КУХНЕ для клиента {context.Message.Message.ClientId} по техническим причинам [!]");

        await _bus.Publish<IKitchenCanceledEvent>(new KitchenCanceledEvent(
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