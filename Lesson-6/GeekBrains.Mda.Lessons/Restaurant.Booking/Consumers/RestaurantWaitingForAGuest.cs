using MassTransit;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantWaitingForAGuest : IConsumer<IBookingApprovedEvent>
{
    /// <summary>
    /// Подписка на ожидание гостя
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <exception cref="Exception">Исключение</exception>
    public async Task Consume(ConsumeContext<IBookingApprovedEvent> context)
    {
        try
        {
            int guestArrivedAtTime = new Random().Next(7_000, 15_000);
            Console.WriteLine($"[X] Ждем гостя: {context.Message.OrderId}]");
            Console.WriteLine($"[X] Пребудет через: {guestArrivedAtTime} времени...");

            await Task.Delay(guestArrivedAtTime);

            await context.Publish<IGuestHasArrived>(new GuestHasArrived(
                context.Message.OrderId,
                context.Message.ClientId,
                guestArrivedAtTime));
        
            await context.ConsumeCompleted;
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex}");
        }
    }
}