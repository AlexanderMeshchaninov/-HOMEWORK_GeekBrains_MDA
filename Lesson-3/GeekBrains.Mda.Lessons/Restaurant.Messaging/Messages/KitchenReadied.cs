using Restaurant.Messaging.MessagesContracts;

namespace Restaurant.Messaging.Messages;

public class KitchenReadied : IKitchenReadied
{
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public int TableNumber { get; set; }
    public Dish? PreOrder { get; set; }
    public string Comments { get; set; }
    public bool Success { get; set; }
}