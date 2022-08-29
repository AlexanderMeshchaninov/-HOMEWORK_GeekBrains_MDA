using MassTransit;
using Mda.Lessons.Core.ApplicationSettings;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Messaging.IdempotentConsumer;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingCancelRequestConsumer : IConsumer<IBookingCancelRequestEvent>
{
    private readonly RestaurantSmsService _smsService;
    private readonly RestaurantBookingStatus _bookingStatus;
    private readonly IMakeIdempotentConsumer _makeIdempotentConsumer;
    private readonly ILogger<RestaurantBookingCancelRequestConsumer> _logger;

    public RestaurantBookingCancelRequestConsumer(
        RestaurantSmsService smsService, 
        RestaurantBookingStatus bookingStatus, 
        IMakeIdempotentConsumer makeIdempotentConsumer, 
        ILogger<RestaurantBookingCancelRequestConsumer> logger)
    {
        _smsService = smsService;
        _bookingStatus = bookingStatus;
        _makeIdempotentConsumer = makeIdempotentConsumer;
        _logger = logger;
    }
    
    /// <summary>
    /// Подписка на событие отмены бронирования столика в ресторане по запросу клиента
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<IBookingCancelRequestEvent> context)
    {
        try
        {
            await _makeIdempotentConsumer.MakeConsumerIdempotentInMemoryAsync(context);

            Table? cancelTable = await _smsService.CancelBookingBySmsAsync(context.Message.TableNumber);

            if (cancelTable is not null)
            {
                _logger.Log(LogLevel.Information, $"[X] Столик номер {cancelTable.TableNumber} " +
                                                  $"снят с брони по запросу {cancelTable.OrderId}" +
                                                  $"\n[X] Trying time: {DateTime.Now}");

                await context.Publish<ITableCanceledEvent>(new TableCanceledEvent(
                    context.Message.OrderId,
                    context.Message.ClientId,
                    context.Message.PreOrder,
                    cancelTable.TableNumber,
                    0,
                    context.Message.CountOfPersons,
                    _bookingStatus.Canceled,
                    true,
                    context.Message.CreationDate));
            }
            else
            {
                _logger.Log(LogLevel.Error, "Object - cancelTable is empty");
                throw new ArgumentNullException("Объект пустой");
            }
        }
        finally
        {
            await context.ConsumeCompleted;
        }
    }
}