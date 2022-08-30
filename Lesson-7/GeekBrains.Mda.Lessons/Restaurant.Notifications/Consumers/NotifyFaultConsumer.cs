using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Notifications.Consumers;

public class NotifyFaultConsumer : IConsumer<Fault<INotifyEvent>>
{
    private readonly ILogger<NotifyConsumer> _logger;

    public NotifyFaultConsumer(ILogger<NotifyConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<Fault<INotifyEvent>> context)
    {
        _logger.Log(LogLevel.Error, "Произошла ошибка при публикации сообщения!");

        await context.ConsumeCompleted;
    }
}