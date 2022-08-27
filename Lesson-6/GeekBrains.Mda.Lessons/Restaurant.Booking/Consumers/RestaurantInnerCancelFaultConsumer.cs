using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantInnerCancelFaultConsumer : IConsumer<Fault<IBookingInnerCancelEvent>>
{
    /// <summary>
    /// Подписка на отмену в зале при возникновении ЧП, срабатывает автоматически если в этот момент произошла ошибка
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task Consume(ConsumeContext<Fault<IBookingInnerCancelEvent>> context)
    {
        Console.WriteLine($"[!] Произошла ошибка при внутренней отмене столика {context.Message.Message.OrderId} [!]");
        //Тут можно обрабатывать ошибки...
        return Task.CompletedTask;
    }
}