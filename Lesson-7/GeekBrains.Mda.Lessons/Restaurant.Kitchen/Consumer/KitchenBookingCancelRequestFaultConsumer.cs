using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenBookingCancelRequestFaultConsumer : IConsumer<Fault<IBookingCancelRequestEvent>>
{
    private readonly ILogger<KitchenBookingCancelRequestFaultConsumer> _logger;

    public KitchenBookingCancelRequestFaultConsumer(ILogger<KitchenBookingCancelRequestFaultConsumer> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Подписка на событие отмены бронирования столика на кухне если возникла ошибка
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <returns></returns>
    public Task Consume(ConsumeContext<Fault<IBookingCancelRequestEvent>> context)
    {
        _logger.Log(LogLevel.Error,$"[!] Произошла ошибка при отмене кухни для клиента " +
                                   $"{context.Message.Message.OrderId} [!]");
        //Тут можно обрабатывать ошибки...
        return context.ConsumeCompleted;
    }
}