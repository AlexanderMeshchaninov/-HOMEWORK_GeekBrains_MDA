using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenInnerCancelConsumer : IConsumer<IBookingInnerCancelEvent>
{
    private readonly ILogger<KitchenInnerCancelConsumer> _logger;

    public KitchenInnerCancelConsumer(ILogger<KitchenInnerCancelConsumer> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Подписка на отмену на кухне при возникновении ЧП, срабатывает автоматически
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Consume(ConsumeContext<IBookingInnerCancelEvent> context)
    {
        try
        {
            if (context.Message.OrderId == Guid.Empty)
            {
                throw new ArgumentException("OrderId is empty");
            }
            else
            {
                _logger.Log(LogLevel.Information,$"[!] Кухня приостановила заказ клиента {context.Message.ClientId} [!]");
            }
        }
        finally
        {
            await context.ConsumeCompleted;
        }
    }
}