using MassTransit;
using Mda.Lessons.Core.Restaurant;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Restaurant;

public class Restaurant : IRestaurant
{
    private readonly RestaurantPhoneService _restaurantPhoneService;
    private readonly RestaurantSmsService _restaurantSmsService;
    private readonly IRestaurantNotificationService _notificationService;
    private readonly IBus _bus;

    public Restaurant (
        IRestaurantNotificationService notificationService, 
        IBus bus, RestaurantPhoneService restaurantPhoneService, 
        RestaurantSmsService restaurantSmsService)
    {
        _notificationService = notificationService;
        _bus = bus;
        _restaurantPhoneService = restaurantPhoneService;
        _restaurantSmsService = restaurantSmsService;
    }
    
    /// <summary>
    /// Бронирование столика через СМС
    /// </summary>
    /// <param name="countOfPersons">Количество запрашиваемых персон</param>
    /// <param name="cancellationToken">Toкен отмены</param>
    public async Task BookingTableBySmsAsync(int countOfPersons, CancellationToken cancellationToken)
    {
        Table bookedTable = await _restaurantSmsService.BookFreeTableBySmsAsync(countOfPersons);
        await _notificationService.NotifyBySmsAsync(bookedTable, cancellationToken);
    }

    /// <summary>
    /// Бронирование столика через Телефон
    /// </summary>
    /// <param name="countOfPersons">Количество запрашиваемых персон</param>
    /// <param name="cancellationToken">Toкен отмены</param>
    public async Task BookingTableByPhoneAsync(int countOfPersons, CancellationToken cancellationToken)
    {
        Table bookedTable = _restaurantPhoneService.BookFreeTableByPhone(countOfPersons);
        await _notificationService.NotifyByPhoneAsync(bookedTable, cancellationToken);
    }

    /// <summary>
    /// Снятие брони через СМС
    /// </summary>
    /// <param name="tableNumber">Номер столика для снятия брони</param>
    /// <param name="cancellationToken">Toкен отмены</param>
    public async Task CancelBookingBySmsAsync(int tableNumber, CancellationToken cancellationToken)
    {
        Table canceledTable = await _restaurantSmsService.CancelBookingBySmsAsync(tableNumber);
        await _notificationService.NotifyCancelBySmsAsync(canceledTable, cancellationToken);
    }

    /// <summary>
    /// Снятие брони через Телефон
    /// </summary>
    /// <param name="tableNumber">Номер столика для снятия брони</param>
    /// <param name="cancellationToken">Toкен отмены</param>
    public async Task CancelBookingByPhoneAsync(int tableNumber, CancellationToken cancellationToken)
    {
        Table canceledTable = _restaurantPhoneService.CancelBookingByPhone(tableNumber);
        await _notificationService.NotifyCancelByPhoneAsync(canceledTable, cancellationToken);
    }

    /// <summary>
    /// Отправка запроса на бронирование столика
    /// </summary>
    /// <param name="countOfPersons">Количество человек</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task RequestBookingTableEventBySmsAsync(int countOfPersons, Dish dishChoice, CancellationToken cancellationToken)
    {
        DateTime creationDate = DateTime.Now;
        await _bus.Publish<IBookingRequestEvent>(new BookingRequestEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            dishChoice,
            0,
            0,
            countOfPersons,
            "Заказ столика",
            true,
            creationDate), cancellationToken);
    }

    /// <summary>
    /// Отправка запроса на бронирование столика
    /// </summary>
    /// <param name="countOfPersons">Количество человек</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task RequestBookingTableEventByPhoneAsync(int countOfPersons, Dish dishChoice, CancellationToken cancellationToken)
    {
        DateTime creationDate = DateTime.Now;
        await _bus.Publish<IBookingRequestEvent>(new BookingRequestEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            dishChoice,
            0,
            0,
            countOfPersons,
            "Заказ столика",
            true,
            creationDate), cancellationToken);
    }

    /// <summary>
    /// Отправка запроса отмены бронирования столика
    /// </summary>
    /// <param name="tableNumber">Номер столика</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task RequestBookingCancelEventBySmsAsync(int tableNumber, CancellationToken cancellationToken)
    {
        DateTime creationDate = DateTime.Now;
        await _bus.Publish<IBookingCancelRequestEvent>(new BookingCancelRequestEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            tableNumber,
            0,
            0,
            "Отмена столика",
            true,
            creationDate), cancellationToken);
    }

    /// <summary>
    /// Отправка запроса отмены бронирования столика
    /// </summary>
    /// <param name="tableNumber">Номер столика</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task RequestBookingCancelEventByPhoneAsync(int tableNumber, CancellationToken cancellationToken)
    {
        DateTime creationDate = DateTime.Now;
        await _bus.Publish<IBookingCancelRequestEvent>(new BookingCancelRequestEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            tableNumber,
            0,
            0,
            "Отмена столика",
            true,
            creationDate), cancellationToken);
    }
}