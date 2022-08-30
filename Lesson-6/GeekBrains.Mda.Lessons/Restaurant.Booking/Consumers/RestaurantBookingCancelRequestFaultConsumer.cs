using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingCancelRequestFaultConsumer : IConsumer<Fault<IBookingInnerCancelEvent>>
{
    /// <summary>
    /// Подписка на событие ошибки при отмене бронирования столика в ресторане по запросу клиента
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <returns></returns>
    public Task Consume(ConsumeContext<Fault<IBookingInnerCancelEvent>> context)
    {
        Console.WriteLine($"[!] Произошла ошибка при отмене столика для клиента {context.Message.Message.OrderId} [!]");
        //Тут можно обрабатывать ошибки...
        return Task.CompletedTask;
    }
}