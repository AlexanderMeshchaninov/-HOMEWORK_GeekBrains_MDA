using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingCancelRequestFaultConsumer : IConsumer<Fault<IBookingCancelRequestEvent>>
{
    private readonly ILogger<RestaurantBookingCancelRequestFaultConsumer> _logger;

    public RestaurantBookingCancelRequestFaultConsumer(ILogger<RestaurantBookingCancelRequestFaultConsumer> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Подписка на событие ошибки при отмене бронирования столика в ресторане по запросу клиента
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <returns></returns>
    public Task Consume(ConsumeContext<Fault<IBookingCancelRequestEvent>> context)
    {
        _logger.Log(LogLevel.Information,$"[!] Произошла ошибка при отмене столика для клиента " +
                                         $"{context.Message.Message.OrderId} [!]");
        //Тут можно обрабатывать ошибки...
        return Task.CompletedTask;
    }
}