using GeekBrains.Mda.Logic.Services;

namespace GeekBrains.Mda.Logic.Restaurant;

public sealed class Restaurant
{
    private readonly RestaurantPhoneService _restaurantPhoneService;
    private readonly RestaurantSmsService _restaurantSmsService;
    private readonly RestaurantNotificationService _notificationService;
    public Restaurant()
    {
        List<Table> tables = new List<Table>();
        for (int i = 1; i <= 10; i++)
        {
            tables.Add(new Table(i));
        }
        
        RestaurantAutoCancelService restaurantAutoCancelService = new RestaurantAutoCancelService(tables, 20_000);
        _restaurantPhoneService = new RestaurantPhoneService(tables);
        _restaurantSmsService = new RestaurantSmsService(tables);
        _notificationService = new RestaurantNotificationService();
    }

    //Ресторан делегирует функции бронирования и снятия брони своему отделу по работе с клиентами
    public async Task BookingTableBySmsAsync(int countOfPersons)
    {
        Table bookedTable = await _restaurantSmsService.BookFreeTableBySmsAsync(countOfPersons);
        await _notificationService.NotifyBySmsAsync(bookedTable);
    }
    
    public async Task CancelBookingBySmsAsync(int tableId)
    {
        Table canceledTable = await _restaurantSmsService.CancelBookingBySmsAsync(tableId);
        await _notificationService.NotifyBySmsAsync(canceledTable);
    }

    public void BookingTableByPhone(int countOfPersons)
    {
        Table bookedTable = _restaurantPhoneService.BookFreeTableByPhone(countOfPersons);
        _notificationService.NotifyByPhone(bookedTable);
    }
    
    public void CancelBookingByPhone(int tableId)
    {
        Table canceledTable = _restaurantPhoneService.CancelBookingByPhone(tableId);
        _notificationService.NotifyCancelByPhone(canceledTable);
    }
}