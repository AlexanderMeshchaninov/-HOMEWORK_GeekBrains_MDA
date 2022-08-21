using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class GuestHasArrived : IGuestHasArrived
{
    public Guid OrderId { get; }
    public int Time { get; }

    public GuestHasArrived(Guid orderId, int time)
    {
        OrderId = orderId;
        Time = time;
    }
}