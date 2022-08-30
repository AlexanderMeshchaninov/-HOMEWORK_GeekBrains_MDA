using MassTransit;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingRequestFaultConsumer : IConsumer<Fault<IBookingRequestEvent>>
{
    private readonly ILogger<RestaurantBookingRequestFaultConsumer> _logger;
    private readonly RestaurantSmsService _smsService;
    public RestaurantBookingRequestFaultConsumer(
        ILogger<RestaurantBookingRequestFaultConsumer> logger, 
        RestaurantSmsService smsService)
    {
        _smsService = smsService;
        _logger = logger;
    }
    
    /// <summary>
    /// Подписка на событие бронирования столика в ресторане при возникновении ошибки
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <exception cref="ArgumentNullException">Если объект столика пустой</exception>
    public async Task Consume(ConsumeContext<Fault<IBookingRequestEvent>> context)
    {
        try
        {
            Table? canceledTable = await _smsService.CancelTableBySmsFromRequestAsync(context.Message.Message.OrderId);
            
            if (canceledTable is not null)
            {
                _logger.Log(LogLevel.Information,$"[!] ОТМЕНА В ЗАЛЕ для клиента " +
                                                 $"{context.Message.Message.ClientId} по техническим причинам [!]");
                await context.ConsumeCompleted;
            }
            else
            {
                _logger.Log(LogLevel.Error,"Object - cancelTable is empty");
                throw new ArgumentNullException("Объект пустой");
            }
        }
        catch (ArgumentNullException ex)
        {
            _logger.Log(LogLevel.Error,$"{ex}");
        }
    }
}