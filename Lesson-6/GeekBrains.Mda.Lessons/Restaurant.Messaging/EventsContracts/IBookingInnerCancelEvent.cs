namespace Restaurant.Messaging.EventsContracts;

public interface IBookingInnerCancelEvent
{
     Guid OrderId { get; }
     Guid ClientId { get; }
}