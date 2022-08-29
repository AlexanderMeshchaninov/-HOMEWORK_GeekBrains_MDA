using MassTransit;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Notifications.Notifiers;

namespace Restaurant.Notifications.Consumers;

public class NotifyConsumer : IConsumer<INotifyEvent>
{
    private readonly Notifier _notifier;
    private readonly ILogger<NotifyConsumer> _logger;

    public NotifyConsumer(
        Notifier notifier, 
        ILogger<NotifyConsumer> logger)
    {
        _notifier = notifier;
        _logger = logger;
    }

    /// <summary>
    /// Подписка на событие нотификации
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Consume(ConsumeContext<INotifyEvent> context)
    {
        try
        {
            if (context.Message.OrderId == Guid.Empty)
            {
                throw new ArgumentException("OrderId is empty");
            }
            else
            {
                _logger.Log(LogLevel.Information,"Notification message has been send...");
                _notifier.Notify(context.Message.OrderId, context.Message.ClientId, context.Message.Message);
            }
        }
        finally
        {
            await context.ConsumeCompleted;
        }
    }
}