using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class GuestHasArrived : IGuestHasArrived
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public int Time { get; }

    public GuestHasArrived(
        Guid orderId, 
        Guid clientId,
        int time)
    {
        OrderId = orderId;
        ClientId = clientId;
        Time = time;
    }
}