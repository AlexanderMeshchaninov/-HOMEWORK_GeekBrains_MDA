using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenBookingRequestFaultConsumer : IConsumer<Fault<IBookingRequestEvent>>
{
    private readonly ILogger<KitchenBookingRequestFaultConsumer> _logger;

    public KitchenBookingRequestFaultConsumer(ILogger<KitchenBookingRequestFaultConsumer> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Подписка на событие бронирования столика на кухне если возникла ошибка
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<Fault<IBookingRequestEvent>> context)
    {
        _logger.Log(LogLevel.Information,$"[!] ОТМЕНА НА КУХНЕ для клиента " +
                                         $"{context.Message.Message.ClientId} по техническим причинам [!]");
        //Тут можно обрабатывать ошибки...
        await context.ConsumeCompleted;
    }
}