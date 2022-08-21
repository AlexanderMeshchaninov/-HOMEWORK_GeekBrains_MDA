using Mda.Lessons.Core.Restaurant;

namespace Restaurant.Messaging.EventsContracts;

public interface ITableCanceledEvent
{
     Guid OrderId { get; }
     Guid ClientId { get; }
     Dish? PreOrder { get; }
     int TableNumber { get; }
     int Time { get; }
     int CountOfPersons { get; }
     string Message { get; }
     bool Ready { get; }
     DateTime CreationDate { get; }
}