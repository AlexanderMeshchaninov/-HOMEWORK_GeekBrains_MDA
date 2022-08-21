using MassTransit;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantWaitingForAGuest : IConsumer<IBookingApprovedEvent>
{
    private readonly IBus _bus;

    public RestaurantWaitingForAGuest(IBus bus)
    {
        _bus = bus;
    }
    
    public async Task Consume(ConsumeContext<IBookingApprovedEvent> context)
    {
        int guestArrivedAtTime = new Random().Next(7_000, 15_000);
        Console.WriteLine($"[X] [Ждем гостя: {context.Message.OrderId}]...");
        Console.WriteLine($"[X] Пребудет через: {guestArrivedAtTime}");

        await Task.Delay(guestArrivedAtTime);

        await context.Publish<IGuestHasArrived>(new GuestHasArrived(
            context.Message.OrderId,
            guestArrivedAtTime));
        
        await context.ConsumeCompleted;
    }
}