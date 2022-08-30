using MassTransit;
using Mda.Lessons.Core.Kitchen;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class TableBookedConsumer : IConsumer<ITableBookedEvent>
{
    private readonly IKitchenManager _kitchenManager;

    public TableBookedConsumer(IKitchenManager kitchenManager)
    {
        _kitchenManager = kitchenManager;
    }
    
    /// <summary>
    /// Метод получения сообщения от ресторана о забронированном столике
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<ITableBookedEvent> context)
    {
        Guid orderId = context.Message.OrderId;
        Guid clientId = context.Message.ClientId;
        int tableNumber = context.Message.TableNumber;
        string comments = context.Message.Message;
        bool success = context.Message.Ready;
        
        await Task.Run(async () =>
        {
            return "[X] --Кухня в данный момент не может выполнить заказ по техническим причинам--";
        });
        
        await _kitchenManager.CheckToKitchenReadyAsync(orderId, clientId, tableNumber, comments, success);
    }
}