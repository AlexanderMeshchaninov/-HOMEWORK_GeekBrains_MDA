using MassTransit;
using Mda.Lessons.Core.Kitchen;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Kitchen.Consumer;

public class TableCanceledConsumer : IConsumer<ITableCanceledEvent>
{
    private readonly IKitchenManager _kitchenManager;

    public TableCanceledConsumer(IKitchenManager kitchenManager)
    {
        _kitchenManager = kitchenManager;
    }
    
    /// <summary>
    /// Метод получения сообщения от ресторана об отмене забронированного столика
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<ITableCanceledEvent> context)
    {
        Guid orderId = context.Message.OrderId;
        Guid clientId = context.Message.ClientId;
        int tableNumber = context.Message.TableNumber;
        string? comments = context.Message.Message;
        bool success = context.Message.Ready;
        
        await _kitchenManager.CheckToKitchenReadyAsync(orderId, clientId, tableNumber, comments, success); 
    }
}