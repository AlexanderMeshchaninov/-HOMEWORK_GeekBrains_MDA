namespace Restaurant.Messaging.MessagesContracts;

public interface ITableBooked
{
    Guid OrderId { get; set; }
    Guid ClientId { get; set; }
    int TableNumber { get; set; }
    Dish? PreOrder { get; set; }
    string Comments { get; set; }
    bool Success { get; set; }
}