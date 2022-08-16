using MassTransit;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Notifications.Notifiers;

namespace Restaurant.Notifications.Consumers;

public class NotifierKitchenConsumer : IConsumer<IKitchenReadyEvent>
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
    public Task Consume(ConsumeContext<IKitchenReadyEvent> context)
    {
        bool result = context.Message.Ready;
         
        _notifier.Accept(
            context.Message.OrderId, 
            result ? Accepted.Kitchen : Accepted.Rejected,
            0,
            context.Message.TableNumber,
            context.Message.Message,
            context.Message.ClientId);
         
        return Task.CompletedTask;
    }
}