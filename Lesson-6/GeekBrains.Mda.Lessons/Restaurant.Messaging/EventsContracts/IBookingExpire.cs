namespace Restaurant.Messaging.EventsContracts;

public interface IBookingExpire
{
     Guid OrderId { get; }
}