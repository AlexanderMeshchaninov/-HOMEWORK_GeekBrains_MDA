using MassTransit;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenBookingCancelRequestConsumer : IConsumer<IBookingCancelRequestEvent>
{
    private readonly ILogger<KitchenBookingCancelRequestConsumer> _logger;

    public KitchenBookingCancelRequestConsumer(ILogger<KitchenBookingCancelRequestConsumer> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Подписка на событие отмены бронирования столика на кухне
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<IBookingCancelRequestEvent> context)
    {
        try
        {
            if (context.Message.OrderId == Guid.Empty)
            {
                throw new ArgumentException("OrderId is empty");
            }
            else
            {
                _logger.Log(LogLevel.Information, $"[!] [OrderId: {context.Message.OrderId} заказ отменен клиентом " +
                                                  $"{context.Message.ClientId}]" +
                                                  $"\n[!] Trying time: {DateTime.Now}");

                await context.Publish<IKitchenCanceledEvent>(new KitchenCanceledEvent(
                    context.Message.OrderId,
                    context.Message.ClientId,
                    0,
                    context.Message.TableNumber,
                    0,
                    context.Message.CountOfPersons,
                    "Кухня отмена",
                    true,
                    context.Message.CreationDate));
            }
        }
        finally
        {
            await context.ConsumeCompleted;
        }
    }
}