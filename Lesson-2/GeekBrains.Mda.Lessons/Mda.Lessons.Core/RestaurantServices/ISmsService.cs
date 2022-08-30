namespace Mda.Lessons.Core.RestaurantServices;

public interface ISmsService<TResponse> where TResponse : class
{
    Task<TResponse> BookFreeTableBySmsAsync(int countOfPersons);
    Task<TResponse> CancelBookingBySmsAsync(int tableId);
}