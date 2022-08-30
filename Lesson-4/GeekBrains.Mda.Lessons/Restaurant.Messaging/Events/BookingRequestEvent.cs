using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Messaging.Events;

public class BookingRequestEvent : IBookingRequestEvent
{
    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish? PreOrder { get; }
    public int TableNumber { get; }
    public int CountOfPersons { get; }
    public DateTime CreationDate { get; }
    
    public BookingRequestEvent(Guid orderId, Guid clientId, Dish preOrder, int tableNumber, int countOfPersons, DateTime creationDate)
    {
        OrderId = orderId;
        ClientId = clientId;
        PreOrder = preOrder;
        TableNumber = tableNumber;
        CountOfPersons = countOfPersons;
        CreationDate = creationDate;
    }
}