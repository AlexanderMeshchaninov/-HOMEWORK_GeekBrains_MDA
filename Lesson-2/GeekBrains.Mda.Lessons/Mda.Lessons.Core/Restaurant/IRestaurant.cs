namespace Mda.Lessons.Core;

public interface IRestaurant
{
    Task BookingTableBySmsAsync(int countOfPersons);
    Task BookingTableByPhoneAsync(int countOfPersons);
    Task CancelBookingBySmsAsync(int tableId);
    Task CancelBookingByPhoneAsync(int tableId);
}