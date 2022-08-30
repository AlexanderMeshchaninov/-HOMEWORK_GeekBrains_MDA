using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class GuestHasArrivedEvent : IGuestHasArrivedEvent
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public int Time { get; }

    public GuestHasArrivedEvent(
        Guid orderId, 
        Guid clientId,
        int time)
    {
        OrderId = orderId;
        ClientId = clientId;
        Time = time;
    }
}