namespace Restaurant.Messaging.EventsContracts;

public interface IBookingRequestEvent
{
     Guid OrderId { get; }
     Guid ClientId { get; }
     Dish? PreOrder { get; }
     int TableNumber { get; }
     int CountOfPersons { get; }
     DateTime CreationDate { get; }
}