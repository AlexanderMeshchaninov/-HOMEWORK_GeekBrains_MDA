using Mda.Lessons.Core.RestaurantServices;
using Mda.Lessons.Dto;
using Restaurant.Booking.Restaurant;
using Restaurant.Messaging.Producers.MassTransit;

namespace Restaurant.Booking.RestaurantServices;

public sealed class RestaurantNotificationService : INotificationService<Table>
{
    private readonly IProducerMassTransit _producerMassTransit;
    private readonly RestaurantBookingStatus _bookingStatus;

    public RestaurantNotificationService(IProducerMassTransit producerMassTransit, RestaurantBookingStatus bookingStatus)
    {
        _producerMassTransit = producerMassTransit;
        _bookingStatus = bookingStatus;
    }
    
    /// <summary>
    /// Отправка сообщения о бронировании через СМС
    /// </summary>
    /// <param name="bookedTable">Зарезирвированный столик</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task NotifyBySmsAsync(Table bookedTable, CancellationToken cancellationToken)
    {
        await _producerMassTransit.PublishToKitchenTableBookedAsync(bookedTable, _bookingStatus.Booked, cancellationToken);
    }

    /// <summary>
    /// Отправка сообщения о снятии брони через СМС
    /// </summary>
    /// <param name="canceledTable">Снятый с бронирования столик</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task NotifyCancelBySmsAsync(Table canceledTable, CancellationToken cancellationToken)
    {
        await _producerMassTransit.PublishToKitchenTableCanceledAsync(canceledTable, _bookingStatus.Canceled, cancellationToken);
    }
    
    /// <summary>
    /// Отправка сообщения о бронировании через Телефон
    /// </summary>
    /// <param name="bookedTable">Зарезирвированный столик</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task NotifyByPhoneAsync(Table bookedTable, CancellationToken cancellationToken)
    {
        await _producerMassTransit.PublishToKitchenTableBookedAsync(bookedTable, _bookingStatus.Booked, cancellationToken);
    }
    
    /// <summary>
    /// Отправка сообщения о снятии брони через Телефон
    /// </summary>
    /// <param name="canceledTable">Снятый с бронирования столик</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task NotifyCancelByPhoneAsync(Table canceledTable, CancellationToken cancellationToken)
    {
        await _producerMassTransit.PublishToKitchenTableCanceledAsync(canceledTable, _bookingStatus.Canceled, cancellationToken);
    }
}