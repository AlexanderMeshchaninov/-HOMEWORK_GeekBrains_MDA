namespace Mda.Lessons.Core.RestaurantServices;

public interface IPhoneService<TResponse> where TResponse : class
{
    TResponse BookFreeTableByPhone(int countOfPersons);
    TResponse CancelBookingByPhone(int tableId);
}