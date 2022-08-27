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
    
    public RestaurantBookingCancelRequestConsumer(
        RestaurantSmsService smsService, 
        RestaurantBookingStatus bookingStatus, 
        IMakeIdempotentConsumer makeIdempotentConsumer)
    {
        _smsService = smsService;
        _bookingStatus = bookingStatus;
        _makeIdempotentConsumer = makeIdempotentConsumer;
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
                Console.WriteLine($"[X] Столик номер {cancelTable.TableNumber} снят с брони по запросу {cancelTable.OrderId}");
                Console.WriteLine($"[X] Trying time: {DateTime.Now}");
            
                await context.Publish<ITableCanceledEvent>(new TableCanceledEvent(
                    cancelTable.OrderId,
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
            else
            {
                throw new ArgumentNullException("Объект пустой");
            }
        }
        catch (ArgumentNullException ex)
        {
            throw new Exception($"{ex}");
        }
    }
}