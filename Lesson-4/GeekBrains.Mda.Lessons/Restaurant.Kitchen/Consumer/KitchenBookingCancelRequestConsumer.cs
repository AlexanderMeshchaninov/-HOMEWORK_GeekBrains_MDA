using MassTransit;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenBookingCancelRequestConsumer : IConsumer<IBookingCancelRequestEvent>
{
    /// <summary>
    /// Подписка на событие отмены бронирования столика на кухне
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<IBookingCancelRequestEvent> context)
    {
        Console.WriteLine($"[!] [OrderId: {context.Message.OrderId} заказ отменен клиентом {context.Message.ClientId}]");
        
        await context.Publish<IKitchenCanceledEvent>(new KitchenCanceledEvent(
            context.Message.OrderId,
            context.Message.ClientId,
            0,
            context.Message.TableNumber,
            "Кухня отмена",
            true));

        await context.ConsumeCompleted;
    }
}