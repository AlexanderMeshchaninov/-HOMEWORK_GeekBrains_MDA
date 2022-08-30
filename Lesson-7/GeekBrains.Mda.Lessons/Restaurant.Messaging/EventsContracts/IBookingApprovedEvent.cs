namespace Restaurant.Messaging.EventsContracts;

public interface IBookingApprovedEvent
{
     Guid OrderId { get; }
     Guid ClientId { get; }
}