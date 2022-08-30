using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.MassTransitSaga;

public class BookingExpire : IBookingExpire
{
    public Guid OrderId => _instance.OrderId;

    private readonly RestaurantBookingState _instance;
    
    public BookingExpire(RestaurantBookingState instance)
    {
        _instance = instance;
    }
}