namespace Mda.Lessons.Core.RestaurantServices;

public interface ISmsService<TTable> where TTable : class
{
    Task<TTable> BookFreeTableBySmsAsync(int countOfPersons);
    Task<TTable> CancelBookingBySmsAsync(int tableNumber);
}