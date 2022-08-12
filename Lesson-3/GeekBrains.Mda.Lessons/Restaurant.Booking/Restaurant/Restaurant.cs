using Mda.Lessons.Core.Restaurant;
using Mda.Lessons.Core.RestaurantServices;
using Mda.Lessons.Dto;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.Producers.MassTransit;

namespace Restaurant.Booking.Restaurant;

public sealed class Restaurant : IRestaurant
{
    private readonly IPhoneService<Table> _receivePhoneFromClient;
    private readonly ISmsService<Table> _receiveSmsFromClient;
    private readonly INotificationService<Table> _notificationService;
    private readonly IProducerMassTransit _producerMassTransit;
    private readonly RestaurantBookingStatus _bookingStatus;
    public Restaurant (
        IProducerMassTransit producerMassTransit, 
        INotificationService<Table> notificationService,
        RestaurantBookingStatus bookingStatus)
    {
        List<Table> tables = new List<Table>();
        
        for (int i = 1; i <= 10; i++)
        {
            tables.Add(new Table(i));
        }
        
        _producerMassTransit = producerMassTransit;
        _notificationService = notificationService;
        _bookingStatus = bookingStatus;
        _receivePhoneFromClient = new ReceivePhoneFromClient(tables, _bookingStatus);
        _receiveSmsFromClient = new ReceiveSmsFromClient(tables, bookingStatus);
        
        _ = new RestaurantAutoCancelService(tables, 100_000, _producerMassTransit, _bookingStatus);
    }
    
    /// <summary>
    /// Бронирование столика через СМС
    /// </summary>
    /// <param name="countOfPersons">Количество запрашиваемых персон</param>
    /// <param name="cancellationToken">Toкен отмены</param>
    public async Task BookingTableBySmsAsync(int countOfPersons, CancellationToken cancellationToken)
    {
        Table bookedTable = await _receiveSmsFromClient.BookFreeTableBySmsAsync(countOfPersons);
        await _notificationService.NotifyBySmsAsync(bookedTable, cancellationToken);
    }

    /// <summary>
    /// Бронирование столика через Телефон
    /// </summary>
    /// <param name="countOfPersons">Количество запрашиваемых персон</param>
    /// <param name="cancellationToken">Toкен отмены</param>
    public async Task BookingTableByPhoneAsync(int countOfPersons, CancellationToken cancellationToken)
    {
        Table bookedTable = _receivePhoneFromClient.BookFreeTableByPhone(countOfPersons);
        await _notificationService.NotifyByPhoneAsync(bookedTable, cancellationToken);
    }

    /// <summary>
    /// Снятие брони через СМС
    /// </summary>
    /// <param name="tableNumber">Номер столика для снятия брони</param>
    /// <param name="cancellationToken">Toкен отмены</param>
    public async Task CancelBookingBySmsAsync(int tableNumber, CancellationToken cancellationToken)
    {
        Table canceledTable = await _receiveSmsFromClient.CancelBookingBySmsAsync(tableNumber);
        await _notificationService.NotifyCancelBySmsAsync(canceledTable, cancellationToken);
    }

    /// <summary>
    /// Снятие брони через Телефон
    /// </summary>
    /// <param name="tableNumber">Номер столика для снятия брони</param>
    /// <param name="cancellationToken">Toкен отмены</param>
    public async Task CancelBookingByPhoneAsync(int tableNumber, CancellationToken cancellationToken)
    {
        Table canceledTable = _receivePhoneFromClient.CancelBookingByPhone(tableNumber);
        await _notificationService.NotifyCancelByPhoneAsync(canceledTable, cancellationToken);
    }
}