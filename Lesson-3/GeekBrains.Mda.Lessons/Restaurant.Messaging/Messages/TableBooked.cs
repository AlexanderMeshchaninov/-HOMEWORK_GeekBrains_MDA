using Restaurant.Messaging.MessagesContracts;

namespace Restaurant.Messaging.Messages;

public class TableBooked : ITableBooked
{
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public int TableNumber { get; set; }
    public Dish? PreOrder { get; set; }
    public string Comments { get; set; }
    public bool Success { get; set; }
}