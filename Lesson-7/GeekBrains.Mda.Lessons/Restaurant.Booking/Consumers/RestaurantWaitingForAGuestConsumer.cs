using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantWaitingForAGuestConsumer : IConsumer<IBookingApprovedEvent>
{
    private readonly ILogger<RestaurantWaitingForAGuestConsumer> _logger;

    public RestaurantWaitingForAGuestConsumer(ILogger<RestaurantWaitingForAGuestConsumer> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Подписка на ожидание гостя
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <exception cref="Exception">Исключение</exception>
    public async Task Consume(ConsumeContext<IBookingApprovedEvent> context)
    {
        try
        {
            if (context.Message.OrderId == Guid.Empty)
            {
                throw new ArgumentException("OrderId is empty");
            }
            else
            {
                int guestArrivedAtTime = new Random().Next(7_000, 15_000);
                _logger.Log(LogLevel.Information, $"[X] Ждем гостя: {context.Message.OrderId}]" +
                                                  $"\n[X] Пребудет через: {guestArrivedAtTime} времени...");
            
                await Task.Delay(guestArrivedAtTime);
            
                await context.Publish<IGuestHasArrivedEvent>(new GuestHasArrivedEvent(
                    context.Message.OrderId,
                    context.Message.ClientId,
                    guestArrivedAtTime));
            }
        }
        finally
        {
            await context.ConsumeCompleted;
        }
    }
}