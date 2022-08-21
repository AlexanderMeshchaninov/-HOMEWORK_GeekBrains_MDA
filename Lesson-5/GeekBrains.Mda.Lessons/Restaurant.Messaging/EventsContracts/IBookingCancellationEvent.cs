namespace Restaurant.Messaging.EventsContracts;

public interface IBookingCancellationEvent
{
     Guid OrderId { get; }
}