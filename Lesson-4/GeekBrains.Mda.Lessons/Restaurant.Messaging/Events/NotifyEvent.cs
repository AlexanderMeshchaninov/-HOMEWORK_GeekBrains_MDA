using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class NotifyEvent : INotifyEvent
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public int TableNumber { get; }
    public int CountOfPersons { get; }
    public int Time { get; }
    public string? Message { get; }
    
    public NotifyEvent(Guid orderId, Guid clientId, int tableNumber, int time, int countOfPersons, string message)
    {
        OrderId = orderId;
        ClientId = clientId;
        TableNumber = tableNumber;
        Time = time;
        CountOfPersons = countOfPersons;
        Message = message;
    }
}