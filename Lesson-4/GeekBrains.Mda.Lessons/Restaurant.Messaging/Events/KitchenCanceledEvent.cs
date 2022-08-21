using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class KitchenCanceledEvent : IKitchenCanceledEvent
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish? PreOrder { get; }
    public int TableNumber { get; }
    public string? Message { get; }
    public bool Ready { get; }
    
    public KitchenCanceledEvent(Guid orderId, Guid clientId, Dish preOrder, int tableNumber, string message, bool ready)
    {
        OrderId = orderId;
        ClientId = clientId;
        PreOrder = preOrder;
        TableNumber = tableNumber;
        Message = message;
        Ready = ready;
    }
}