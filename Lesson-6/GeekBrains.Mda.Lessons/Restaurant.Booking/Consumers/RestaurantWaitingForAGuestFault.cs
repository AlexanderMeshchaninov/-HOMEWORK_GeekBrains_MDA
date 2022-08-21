using MassTransit;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantWaitingForAGuestFault : IConsumer<Fault<IBookingApprovedEvent>>
{
    private readonly RestaurantSmsService _smsService;

    public RestaurantWaitingForAGuestFault(RestaurantSmsService smsService)
    {
        _smsService = smsService;
    }
    
    /// <summary>
    /// Подписка на ожидание гостя, если в этот момент произошла ошибка
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <exception cref="ArgumentNullException">Если объект столика пустой</exception>
    public async Task Consume(ConsumeContext<Fault<IBookingApprovedEvent>> context)
    {
        try
        {
            Table? canceledTable = await _smsService.CancelTableBySmsFromRequestAsync(context.Message.Message.OrderId);

            if (canceledTable is not null)
            {
                Console.WriteLine($"[!] Произошла ошибка при ожидании клиента {context.Message.Message.ClientId} [!]");
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