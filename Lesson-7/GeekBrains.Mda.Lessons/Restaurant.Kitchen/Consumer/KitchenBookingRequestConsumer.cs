using MassTransit;
using Mda.Lessons.Core.Kitchen;
using Mda.Lessons.Core.Restaurant;
using Microsoft.Extensions.Logging;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class KitchenBookingRequestConsumer : IConsumer<IBookingRequestEvent>
{
    private readonly IKitchenManager _kitchenManager;
    private readonly ILogger<KitchenBookingRequestConsumer> _logger;

    public KitchenBookingRequestConsumer(
        IKitchenManager kitchenManager, 
        ILogger<KitchenBookingRequestConsumer> logger)
    {
        _kitchenManager = kitchenManager;
        _logger = logger;
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

            _logger.Log(LogLevel.Information, $"[X] OrderId: {context.Message.OrderId}" +
                                              $"\n[X] CreationDate: {context.Message.CreationDate}" +
                                              $"\n[X] Проверка на КУХНИ займет {rnd} времени...");
            await Task.Delay(rnd);

            if (context.Message.PreOrder is Dish.Lasagna)
            {
                _logger.Log(LogLevel.Error, "Lasagna is in the stop list! Sorry!");
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
            }
        }
        finally
        {
            await context.ConsumeCompleted;
        }
    }
}