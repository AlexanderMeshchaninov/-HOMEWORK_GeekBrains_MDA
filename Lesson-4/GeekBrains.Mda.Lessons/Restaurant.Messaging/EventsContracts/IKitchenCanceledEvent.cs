namespace Restaurant.Messaging.EventsContracts;

public interface IKitchenCanceledEvent
{
     Guid OrderId { get; }
     Guid ClientId { get; }
     Dish? PreOrder { get; }
     int TableNumber { get; }
     string? Message { get; }
     bool Ready { get; }
}