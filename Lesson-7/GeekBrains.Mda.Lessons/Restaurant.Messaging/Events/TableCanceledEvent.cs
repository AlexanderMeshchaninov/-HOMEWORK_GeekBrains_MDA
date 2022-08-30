using Mda.Lessons.Core.Restaurant;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class TableCanceledEvent : ITableCanceledEvent
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish? PreOrder { get; }
    public int TableNumber { get; }
    public int Time { get; }
    public int CountOfPersons { get; }
    public string? Message { get; }
    public bool Ready { get; }
    public DateTime CreationDate { get; }

    public TableCanceledEvent(    
        Guid orderId, 
        Guid clientId, 
        Dish preOrder, 
        int tableNumber,
        int time,
        int countOfPersons,
        string? message,
        bool ready,
        DateTime creationDate)
    {
        OrderId = orderId;
        ClientId = clientId;
        PreOrder = preOrder;
        TableNumber = tableNumber;
        Time = time;
        CountOfPersons = countOfPersons;
        Message = message;
        Ready = ready;
        CreationDate = creationDate;
    }
}