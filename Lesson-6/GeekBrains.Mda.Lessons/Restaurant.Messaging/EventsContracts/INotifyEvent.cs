namespace Restaurant.Messaging.EventsContracts;

public interface INotifyEvent
{ 
    Guid OrderId { get; }
    Guid ClientId { get; }
    string Message { get; }
}