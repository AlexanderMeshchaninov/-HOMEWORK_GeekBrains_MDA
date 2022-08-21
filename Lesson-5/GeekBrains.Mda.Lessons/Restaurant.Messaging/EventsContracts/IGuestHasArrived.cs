namespace Restaurant.Messaging.EventsContracts;

public interface IGuestHasArrived
{
     Guid OrderId { get; }
     Guid ClientId { get; }
     int Time { get; }
}