namespace Restaurant.Messaging.EventsContracts;

public interface IGuestHasArrivedEvent
{
     Guid OrderId { get; }
     Guid ClientId { get; }
     int Time { get; }
}