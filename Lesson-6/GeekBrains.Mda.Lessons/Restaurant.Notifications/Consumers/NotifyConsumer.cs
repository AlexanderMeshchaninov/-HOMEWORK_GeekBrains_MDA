using MassTransit;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Notifications.Notifiers;

namespace Restaurant.Notifications.Consumers;

public class NotifyConsumer : IConsumer<INotifyEvent>
{
    private readonly NewNotifier _notifier;

    public NotifyConsumer(NewNotifier notifier)
    {
        _notifier = notifier;
    }

    /// <summary>
    /// Подписка на событие нотификации
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task Consume(ConsumeContext<INotifyEvent> context)
    {
        _notifier.Notify(context.Message.OrderId, context.Message.ClientId, context.Message.Message);

        return context.ConsumeCompleted;
    }
}