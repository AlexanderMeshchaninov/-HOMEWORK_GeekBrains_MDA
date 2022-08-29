namespace Mda.Lessons.Core.Restaurant;

public interface IRestaurant
{
    Task RequestBookingTableEventBySmsAsync(int countOfPersons, Dish dishChoice, CancellationToken cancellationToken);
    Task RequestBookingTableEventByPhoneAsync(int countOfPersons, Dish dishChoice, CancellationToken cancellationToken);
    Task RequestBookingCancelEventBySmsAsync(int tableNumber, CancellationToken cancellationToken);
    Task RequestBookingCancelEventByPhoneAsync(int tableNumber, CancellationToken cancellationToken);
}