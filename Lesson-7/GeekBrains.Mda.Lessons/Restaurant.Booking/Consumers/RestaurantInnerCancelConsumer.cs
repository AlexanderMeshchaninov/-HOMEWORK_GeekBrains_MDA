using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Messaging.IdempotentConsumer;

namespace Restaurant.Booking.Consumers;

public class RestaurantInnerCancelConsumer : IConsumer<IBookingInnerCancelEvent>
{
    private readonly RestaurantSmsService _smsService;
    private readonly IMakeIdempotentConsumer _makeIdempotentConsumer;
    private readonly ILogger<RestaurantInnerCancelConsumer> _logger;
 
    public RestaurantInnerCancelConsumer(
        RestaurantSmsService smsService, 
        IMakeIdempotentConsumer makeIdempotentConsumer, 
        ILogger<RestaurantInnerCancelConsumer> logger)
    {
        _smsService = smsService;
        _makeIdempotentConsumer = makeIdempotentConsumer;
        _logger = logger;
    }
    
    /// <summary>
    /// Подписка на отмену в зале при возникновении ЧП, срабатывает автоматически
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <exception cref="ArgumentNullException">Если объект столика пустой</exception>
    public async Task Consume(ConsumeContext<IBookingInnerCancelEvent> context)
    {
        try
        {
            await _makeIdempotentConsumer.MakeConsumerIdempotentInMemoryAsync(context);

            Table? canceledTable = await _smsService.CancelTableBySmsFromRequestAsync(context.Message.OrderId);

            if (canceledTable is not null)
            {
                _logger.Log(LogLevel.Information,
                    $"[!] Зал снял бронь столика для клиента {context.Message.ClientId} [!]" +
                    $"\n[!] Номер столика: {canceledTable.TableNumber} по запросу системы [!]");
            }
            else
            {
                _logger.Log(LogLevel.Error, "Object - canceledTable is empty");
                throw new ArgumentNullException("Объект пустой");
            }
        }
        finally
        {
            await context.ConsumeCompleted;
        }
    }
}