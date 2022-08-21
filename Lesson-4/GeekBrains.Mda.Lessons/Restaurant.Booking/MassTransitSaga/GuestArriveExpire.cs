using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.MassTransitSaga;

public class GuestArriveExpire : IGuestArriveExpire
{
    public Guid OrderId => _instance.OrderId;

    private readonly RestaurantBookingState _instance;
    
    public GuestArriveExpire(RestaurantBookingState instance)
    {
        _instance = instance;
    }
}