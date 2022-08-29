using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantInnerCancelFaultConsumer : IConsumer<Fault<IBookingInnerCancelEvent>>
{
    private readonly ILogger<RestaurantInnerCancelFaultConsumer> _logger;
    public RestaurantInnerCancelFaultConsumer(ILogger<RestaurantInnerCancelFaultConsumer> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Подписка на отмену в зале при возникновении ЧП, срабатывает автоматически если в этот момент произошла ошибка
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task Consume(ConsumeContext<Fault<IBookingInnerCancelEvent>> context)
    {
        _logger.Log(LogLevel.Error,$"[!] Произошла ошибка при внутренней отмене столика " +
                                   $"{context.Message.Message.OrderId} [!]");
        //Тут можно обрабатывать ошибки...
        return Task.CompletedTask;
    }
}