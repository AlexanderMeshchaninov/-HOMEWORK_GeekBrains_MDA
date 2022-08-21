using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class KitchenReadyEvent : IKitchenReadyEvent
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public int TableNumber { get; }
    public Dish? PreOrder { get; }
    public string? Message { get; }
    public bool Ready { get; }

    public KitchenReadyEvent(Guid orderId, Guid clientId, Dish preOrder, int tableNumber, string message, bool ready)
    {
        OrderId = orderId;
        ClientId = clientId;
        PreOrder = preOrder;
        TableNumber = tableNumber;
        Message = message;
        Ready = ready;
    }
}