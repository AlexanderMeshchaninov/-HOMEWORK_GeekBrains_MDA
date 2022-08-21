namespace Restaurant.Messaging.EventsContracts;

public interface ITableCanceledEvent
{
     Guid OrderId { get; }
     Guid ClientId { get; }
     int TableNumber { get; }
     Dish? PreOrder { get; set; }
     string? Message { get; }
     bool Ready { get; }
}