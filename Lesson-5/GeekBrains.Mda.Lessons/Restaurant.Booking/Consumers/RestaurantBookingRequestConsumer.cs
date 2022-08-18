using MassTransit;
using Mda.Lessons.Core.ApplicationSettings;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequestEvent>
{
    private readonly RestaurantSmsService _smsService;
    private readonly RestaurantBookingStatus _bookingStatus;

    public RestaurantBookingRequestConsumer(RestaurantBookingStatus bookingStatus, RestaurantSmsService smsService)
    {
        _bookingStatus = bookingStatus;
        _smsService = smsService;
    }

    /// <summary>
    /// Подписка на событие бронирования столика в ресторане
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<IBookingRequestEvent> context)
    {
        try
        {
            Table bookedTable = await _smsService.BookFreeTableBySmsAsync(context.Message.CountOfPersons);
            Console.WriteLine($"[X] Столик номер [{bookedTable.TableNumber}] ЗАБРОНИРОВАН для клиента {context.Message.ClientId}");
            Console.WriteLine($"[X] Trying time: {DateTime.Now}");
            
            await context.Publish<ITableBookedEvent>(new TableBookedEvent(
                context.Message.OrderId,
                context.Message.ClientId,
                context.Message.PreOrder,
                bookedTable.TableNumber,
                0,
                context.Message.CountOfPersons,
                _bookingStatus.Booked,
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