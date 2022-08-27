using MassTransit;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingRequestFaultConsumer : IConsumer<Fault<IBookingRequestEvent>>
{
    private readonly IBus _bus;
    private readonly RestaurantSmsService _smsService;
    public RestaurantBookingRequestFaultConsumer(IBus bus, RestaurantSmsService smsService)
    {
        _bus = bus;
        _smsService = smsService;
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
                Console.WriteLine($"[!] ОТМЕНА В ЗАЛЕ для клиента {context.Message.Message.ClientId} по техническим причинам [!]");
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