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
        try
        {
            Table cancelTable = await _smsService.CancelBookingBySmsAsync(context.Message.TableNumber);
            Console.WriteLine($"[X] Столик номер {cancelTable.TableNumber} снят с брони по запросу клиента {context.Message.ClientId}");
            Console.WriteLine($"[X] Trying time: {DateTime.Now}");
            
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

            await context.ConsumeCompleted;
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex}");
        }
        
    }
}