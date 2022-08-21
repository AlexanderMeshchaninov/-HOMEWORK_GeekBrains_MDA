using MassTransit;
using Mda.Lessons.Core.ApplicationSettings;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingCancelRequestConsumer : IConsumer<IBookingCancelRequestEvent>
{
    private readonly RestaurantSmsService _smsService;
    private readonly RestaurantBookingStatus _bookingStatus;
    
    public RestaurantBookingCancelRequestConsumer(RestaurantSmsService smsService, RestaurantBookingStatus bookingStatus)
    {
        _smsService = smsService;
        _bookingStatus = bookingStatus;
    }
    
    /// <summary>
    /// Подписка на событие отмены бронирования столика в ресторане
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<IBookingCancelRequestEvent> context)
    {
        Table cancelTable = await _smsService.CancelBookingBySmsAsync(context.Message.TableNumber);
        Console.WriteLine($"[X] Столик номер {cancelTable.TableNumber} снят с брони по запросу клиента: [OrderId: {context.Message.ClientId}]");

        await context.Publish<ITableCanceledEvent>(new TableCanceledEvent(
            context.Message.OrderId,
            context.Message.ClientId,
            0,
            0,
            "Столик снят с брони",
            true));

        await context.ConsumeCompleted;
    }
}