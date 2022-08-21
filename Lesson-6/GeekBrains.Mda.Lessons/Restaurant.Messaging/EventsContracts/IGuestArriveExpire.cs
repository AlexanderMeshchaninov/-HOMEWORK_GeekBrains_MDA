namespace Restaurant.Messaging.EventsContracts;

public interface IGuestArriveExpire
{
     Guid OrderId { get; }
}