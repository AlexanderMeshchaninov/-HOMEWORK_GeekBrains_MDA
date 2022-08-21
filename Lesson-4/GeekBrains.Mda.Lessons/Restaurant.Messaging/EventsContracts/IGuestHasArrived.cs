namespace Restaurant.Messaging.EventsContracts;

public interface IGuestHasArrived
{
     Guid OrderId { get; }
     int Time { get; }
}