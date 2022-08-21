namespace Restaurant.Messaging.EventsContracts;

public interface INotifyEvent
{ 
    Guid OrderId { get; } 
    Guid ClientId { get; } 
    int TableNumber { get; } 
    int CountOfPersons { get; }
    int Time { get; } 
    string? Message { get; }
}