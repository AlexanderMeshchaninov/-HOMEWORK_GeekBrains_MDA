using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class BookingCancellationEvent : IBookingCancellationEvent
{
    public Guid OrderId { get; }

    public BookingCancellationEvent(Guid orderId)
    {
        OrderId = orderId;
    }
}