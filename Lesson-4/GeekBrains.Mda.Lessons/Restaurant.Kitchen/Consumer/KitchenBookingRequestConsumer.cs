using MassTransit;
using Mda.Lessons.Core.Kitchen;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenBookingRequestConsumer : IConsumer<IBookingRequestEvent>
{
    private readonly IKitchenManager _manager;

    public KitchenBookingRequestConsumer(IKitchenManager manager)
    {
        _manager = manager;
    }
    
    /// <summary>
    /// Подписка на событие бронирования столика на кухне
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<IBookingRequestEvent> context)
    {
        try
        {
            Console.WriteLine($"[X] [OrderId: {context.Message.OrderId} CreationDate: {context.Message.CreationDate}]");
            Console.WriteLine("[X] Trying time: " + DateTime.Now);
        
            int rnd = new Random().Next(1_000, 10_000);

            Console.WriteLine($"[X] Проверка на кухни займет... {rnd}");
            await Task.Delay(rnd);
        
            await context.Publish<IKitchenReadyEvent>(new KitchenReadyEvent(
                context.Message.OrderId,
                context.Message.ClientId,
                0,
                context.Message.TableNumber,
                "Кухня готова",
                true));
        
            await context.ConsumeCompleted;
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex}");
        }
    }
}