using MassTransit;
using Mda.Lessons.Core.Producers;
using Mda.Lessons.Core.Restaurant;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Producers.MassTransit;

public interface IProducerMassTransit : 
    IRestaurantProducerMassTransit<TableBase>,
    IKitchenProducerMassTransit
{ }

public class ProducerMassTransit : IProducerMassTransit
{
    private readonly IBus _bus;

    public ProducerMassTransit(IBus bus)
    {
        _bus = bus;
    }
    
    /// <summary>
    /// Метод публикации ресторана для кухни о том, что заказ сделан
    /// </summary>
    /// <param name="tableInfo">Информация о заказанном столике</param>
    /// <param name="message">Комментарии к заказу</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task PublishToKitchenTableBookedAsync(TableBase tableInfo, string message, CancellationToken cancellationToken)
    {
        bool result = tableInfo is not null ? true : false;
        
        await _bus.Publish<ITableBookedEvent>(new TableBookedEvent(
            tableInfo.OrderId, 
            tableInfo.ClientId, 
            0, 
            tableInfo.TableNumber,
            0, 
            0,
            message,
             true,
            DateTime.Now), cancellationToken);
    }

    /// <summary>
    /// Метод публикации ресторана для кухни о том, что заказ отменен
    /// </summary>
    /// <param name="tableInfo">Информация об отмененном столике</param>
    /// <param name="message">Комментарии к отмене</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task PublishToKitchenTableCanceledAsync(TableBase tableInfo, string message, CancellationToken cancellationToken)
    {
        bool result = tableInfo is not null ? true : false;
        
        if (result)
        {
            await _bus.Publish<ITableCanceledEvent>(new TableCanceledEvent(
                tableInfo.OrderId, 
                tableInfo.ClientId, 
                0, 
                tableInfo.TableNumber,
                0, 
                0,
                message,
                true,
                DateTime.Now), cancellationToken);
        }
    }

    /// <summary>
    /// Метод публикации в шину сообщений о готовности кухни
    /// </summary>
    /// <param name="orderId">Номер заказа</param>
    /// <param name="clientId">Номер клиента</param>
    /// <param name="tableNumber">Номер заказанного столика</param>
    /// <param name="message">Комментарии к заказу</param>
    /// <param name="ready">Статус кухня работает или нет</param>
    public async Task PublishToNotificationsAsync(Guid orderId, Guid clientId, int tableNumber, string message, bool ready)
    {
        if (ready)
        {
            await _bus.Publish<IKitchenReadyEvent>(new KitchenReadyEvent(
                orderId, 
                clientId, 
                0, 
                tableNumber,
                0, 
                0,
                message,
                ready,
                DateTime.Now));

            Console.WriteLine($"[X] Кухня выполняет заказ клиента {clientId}, " +
                              $"\n[X] Статус: '{message}' ------>");
        }
        else
        {
            await _bus.Publish<IKitchenReadyEvent>(new KitchenReadyEvent(
                orderId, 
                clientId, 
                0, 
                tableNumber,
                0, 
                0,
                message,
                false,
                DateTime.Now));
        
            Console.WriteLine($"[X] Кухня прекращает заказ клиента {clientId}, " +
                              $"\n[X] Статус: '{message}' ------>");
        }
    }
}