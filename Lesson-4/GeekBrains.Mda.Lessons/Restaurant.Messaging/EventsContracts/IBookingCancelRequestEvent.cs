namespace Restaurant.Messaging.EventsContracts;

public interface IBookingCancelRequestEvent
{
     Guid OrderId { get; }
     Guid ClientId { get; }
     Dish? PreOrder { get; }
     int TableNumber { get; }
     int CountOfPersons { get; }
     DateTime CreationDate { get; }
}