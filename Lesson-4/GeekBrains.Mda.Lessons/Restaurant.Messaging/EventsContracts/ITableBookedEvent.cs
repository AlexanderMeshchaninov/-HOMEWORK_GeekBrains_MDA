namespace Restaurant.Messaging.EventsContracts;

public interface ITableBookedEvent
{
     Guid OrderId { get; }
     Guid ClientId { get; }
     int TableNumber { get; }
     Dish? PreOrder { get; set; }
     string? Message { get; }
     bool Ready { get; }
}