using MassTransit;
using Mda.Lessons.Core.Kitchen;
using Restaurant.Messaging.MessagesContracts;

namespace Restaurant.Kitchen.Consumer;

public sealed class TableCanceledConsumer : IConsumer<ITableCanceled>
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
    public async Task Consume(ConsumeContext<ITableCanceled> context)
    {
        Guid orderId = context.Message.OrderId;
        Guid clientId = context.Message.ClientId;
        int tableNumber = context.Message.TableNumber;
        string comments = context.Message.Comments;
        bool success = context.Message.Success;
        
        await _kitchenManager.CheckToKitchenReadyAsync(orderId, clientId, tableNumber, comments, success); 
    }
}