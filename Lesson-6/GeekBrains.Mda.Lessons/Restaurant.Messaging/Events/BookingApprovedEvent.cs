using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class BookingApprovedEvent : IBookingApprovedEvent
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }

    public BookingApprovedEvent(Guid orderId, Guid clientId)
    {
        OrderId = orderId;
        ClientId = clientId;
    }
}