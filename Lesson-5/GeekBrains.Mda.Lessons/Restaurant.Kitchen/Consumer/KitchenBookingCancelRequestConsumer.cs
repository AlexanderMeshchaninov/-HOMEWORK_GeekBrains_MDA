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
        try
        {
            Console.WriteLine($"[!] [OrderId: {context.Message.OrderId} заказ отменен клиентом {context.Message.ClientId}]");
            Console.WriteLine($"[!] Trying time: {DateTime.Now}");
        
            await context.Publish<IKitchenCanceledEvent>(new KitchenCanceledEvent(
                context.Message.OrderId,
                context.Message.ClientId,
                0,
                context.Message.TableNumber,
                0,
                context.Message.CountOfPersons,
                "Кухня отмена",
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