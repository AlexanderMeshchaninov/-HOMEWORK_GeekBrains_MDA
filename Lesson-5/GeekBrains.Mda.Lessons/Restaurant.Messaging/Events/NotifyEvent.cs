using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class NotifyEvent : INotifyEvent
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public string Message { get; }

    public NotifyEvent(
        Guid orderId,
        Guid clientId,
        string message)
    {
        OrderId = orderId;
        ClientId = clientId;
        Message = message;
    }
}