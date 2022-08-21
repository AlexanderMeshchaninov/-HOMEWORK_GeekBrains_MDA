using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class TableBookedEvent : ITableBookedEvent
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public int TableNumber { get; }
    public Dish? PreOrder { get; set; }
    public string Message { get; }
    public bool Ready { get; }

    public TableBookedEvent(Guid orderId, Guid clientId, int tableNumber, Dish preOrder, string message, bool ready)
    { 
        OrderId = orderId; 
        ClientId = clientId;
        TableNumber = tableNumber;
        PreOrder = preOrder;
        Message = message;
        Ready = ready;
    }
}