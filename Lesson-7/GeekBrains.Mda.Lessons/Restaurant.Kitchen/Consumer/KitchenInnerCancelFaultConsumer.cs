using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenInnerCancelFaultConsumer : IConsumer<Fault<IBookingInnerCancelEvent>>
{
    private readonly ILogger<KitchenInnerCancelFaultConsumer> _logger;

    public KitchenInnerCancelFaultConsumer(ILogger<KitchenInnerCancelFaultConsumer> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Подписка на отмену на кухне при возникновении ЧП, срабатывает автоматически если при этом возникла ошибка
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task Consume(ConsumeContext<Fault<IBookingInnerCancelEvent>> context)
    {
        _logger.Log(LogLevel.Information,$"[!] Произошла ошибка при отмене кухни для клиента {context.Message.Message.OrderId} [!]");
        //Тут можно обрабатывать ошибки...
        return context.ConsumeCompleted;
    }
}