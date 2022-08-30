namespace Mda.Lessons.Core.Restaurant;

public interface IRestaurant
{
    Task BookingTableBySmsAsync(int countOfPersons, CancellationToken cancellationToken);
    Task BookingTableByPhoneAsync(int countOfPersons, CancellationToken cancellationToken);
    Task CancelBookingBySmsAsync(int tableNumber, CancellationToken cancellationToken);
    Task CancelBookingByPhoneAsync(int tableNumber, CancellationToken cancellationToken);
    
    Task RequestBookingTableEventBySmsAsync(int countOfPersons, Dish dishChoice, CancellationToken cancellationToken);
    Task RequestBookingTableEventByPhoneAsync(int countOfPersons, Dish dishChoice, CancellationToken cancellationToken);
    Task RequestBookingCancelEventBySmsAsync(int tableNumber, CancellationToken cancellationToken);
    Task RequestBookingCancelEventByPhoneAsync(int tableNumber, CancellationToken cancellationToken);
}