using MassTransit;
using Mda.Lessons.Core.Kitchen;
using Mda.Lessons.Core.Restaurant;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenBookingRequestConsumer : IConsumer<IBookingRequestEvent>
{
    private readonly IKitchenManager _kitchenManager;

    public KitchenBookingRequestConsumer(IKitchenManager kitchenManager)
    {
        _kitchenManager = kitchenManager;
    }
    
    /// <summary>
    /// Подписка на событие бронирования столика на кухне
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<IBookingRequestEvent> context)
    {
        try
        {
            int rnd = new Random().Next(1_000, 10_000);

            Console.WriteLine(
                $"[X] OrderId: {context.Message.OrderId} \n[X] CreationDate: {context.Message.CreationDate}");
            Console.WriteLine($"[X] Проверка на КУХНИ займет {rnd} времени...");
            await Task.Delay(rnd);

            if (context.Message.PreOrder is Dish.Lasagna)
            {
                throw new ArgumentException("Лазанья в стоп листе, пардон!");
            }

            if (await _kitchenManager.CheckIsKitchenReadyAsync())
            {
                await context.Publish<IKitchenReadyEvent>(new KitchenReadyEvent(
                    context.Message.OrderId,
                    context.Message.ClientId,
                    context.Message.PreOrder,
                    context.Message.TableNumber,
                    rnd,
                    context.Message.CountOfPersons,
                    "Кухня готова",
                    true,
                    context.Message.CreationDate));

                await context.ConsumeCompleted;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"{ex}");
        }
    }
}