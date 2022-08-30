using MassTransit;
using Mda.Lessons.Core.Kitchen;
using Restaurant.Messaging.MessagesContracts;

namespace Restaurant.Kitchen.Consumer;

public sealed class TableBookedConsumer : IConsumer<ITableBooked>
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
    public async Task Consume(ConsumeContext<ITableBooked> context)
    {
        Guid orderId = context.Message.OrderId;
        Guid clientId = context.Message.ClientId;
        int tableNumber = context.Message.TableNumber;
        string comments = context.Message.Comments;
        bool success = context.Message.Success;
        
        await Task.Run(async () =>
        {
            return "[X] --Кухня в данный момент не может выполнить заказ по техническим причинам--";
        });
        
        await _kitchenManager.CheckToKitchenReadyAsync(orderId, clientId, tableNumber, comments, success);
    }
}