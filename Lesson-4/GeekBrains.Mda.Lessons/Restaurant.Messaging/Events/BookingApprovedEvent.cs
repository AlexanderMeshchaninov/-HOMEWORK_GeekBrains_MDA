using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class BookingApprovedEvent : IBookingApprovedEvent
{
    public Guid OrderId { get; }

    public BookingApprovedEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}