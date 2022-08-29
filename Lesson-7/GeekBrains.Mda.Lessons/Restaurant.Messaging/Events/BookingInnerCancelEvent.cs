using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class BookingInnerCancelEvent : IBookingInnerCancelEvent
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }

    public BookingInnerCancelEvent(Guid orderId, Guid clientId)
    {
        OrderId = orderId;
        ClientId = clientId;
    }
}