namespace Mda.Lessons.Core.Restaurant;

public interface IRestaurant
{
    Task BookingTableBySmsAsync(int countOfPersons, CancellationToken cancellationToken);
    Task BookingTableByPhoneAsync(int countOfPersons, CancellationToken cancellationToken);
    Task CancelBookingBySmsAsync(int tableNumber, CancellationToken cancellationToken);
    Task CancelBookingByPhoneAsync(int tableNumber, CancellationToken cancellationToken);
}