namespace Mda.Lessons.Core.RestaurantServices;

public interface IPhoneService<TTable> where TTable : class
{
    TTable BookFreeTableByPhone(int countOfPersons);
    TTable CancelBookingByPhone(int tableNumber);
}