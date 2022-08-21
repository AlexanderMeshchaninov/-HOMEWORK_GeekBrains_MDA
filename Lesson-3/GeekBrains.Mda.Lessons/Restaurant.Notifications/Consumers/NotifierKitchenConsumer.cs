using MassTransit;
using Restaurant.Messaging.MessagesContracts;
using Restaurant.Notifications.Notifiers;

namespace Restaurant.Notifications.Consumers;

public sealed class NotifierKitchenConsumer : IConsumer<IKitchenReadied>
{
    private readonly Notifier _notifier;
    
    public NotifierKitchenConsumer(Notifier notifier)
    {
        _notifier = notifier;
    }
    
    /// <summary>
    /// Метод получения сообщения от кухни
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <returns></returns>
    public Task Consume(ConsumeContext<IKitchenReadied> context)
    {
        bool result = context.Message.Success;
         
        _notifier.Accept(
            context.Message.OrderId, 
            result ? Accepted.Kitchen : Accepted.Rejected,
            context.Message.TableNumber,
            context.Message.Comments,
            context.Message.ClientId);
         
        return Task.CompletedTask;
    }
}