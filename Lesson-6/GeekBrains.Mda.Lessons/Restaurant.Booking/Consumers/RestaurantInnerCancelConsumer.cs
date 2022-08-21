using MassTransit;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Messaging.IdempotentConsumer;

namespace Restaurant.Booking.Consumers;

public class RestaurantInnerCancelConsumer : IConsumer<IBookingInnerCancelEvent>
{
    private readonly RestaurantSmsService _smsService;
    private readonly IMakeIdempotentConsumer _makeIdempotentConsumer;

    public RestaurantInnerCancelConsumer(
        RestaurantSmsService smsService, 
        IMakeIdempotentConsumer makeIdempotentConsumer)
    {
        _smsService = smsService;
        _makeIdempotentConsumer = makeIdempotentConsumer;
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
                Console.WriteLine($"[!] Зал снял бронь столика для клиента {context.Message.ClientId} [!]");
                Console.WriteLine($"[!] Номер столика: {canceledTable.TableNumber} по запросу системы [!]");
                await context.ConsumeCompleted;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"{ex}");
        }
    }
}