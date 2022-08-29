using MassTransit;
using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Restaurant;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Restaurant;

public class Restaurant : IRestaurant
{
    private readonly RestaurantBookingStatus _bookingStatus;
    private readonly IBus _bus;
    private readonly ILogger<Restaurant> _logger;

    public Restaurant (
        RestaurantBookingStatus bookingStatus,
        IBus bus, 
        ILogger<Restaurant> logger)
    {
        _bookingStatus = bookingStatus;
        _bus = bus;
        _logger = logger;
    }
    
    /// <summary>
    /// Отправка запроса на бронирование столика
    /// </summary>
    /// <param name="countOfPersons">Количество человек</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task RequestBookingTableEventBySmsAsync(int countOfPersons, Dish dishChoice, CancellationToken cancellationToken)
    {
        if (countOfPersons > 0)
        {
            DateTime creationDate = DateTime.Now;
        
            _logger.Log(LogLevel.Information,$"Booking request to book table has been send to consumer at {creationDate}");
        
            await _bus.Publish<IBookingRequestEvent>(new BookingRequestEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                dishChoice,
                0,
                0,
                countOfPersons,
                _bookingStatus.Booked,
                true,
                creationDate), cancellationToken);
        }
        else
        {
            Console.WriteLine("Количество людей не может быть отрицательным!");
        }
    }

    /// <summary>
    /// Отправка запроса на бронирование столика
    /// </summary>
    /// <param name="countOfPersons">Количество человек</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task RequestBookingTableEventByPhoneAsync(int countOfPersons, Dish dishChoice, CancellationToken cancellationToken)
    {
        if (countOfPersons > 0)
        {
            DateTime creationDate = DateTime.Now;
        
            _logger.Log(LogLevel.Information,$"Booking request to book table has been send to consumer at {creationDate}");
        
            await _bus.Publish<IBookingRequestEvent>(new BookingRequestEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                dishChoice,
                0,
                0,
                countOfPersons,
                _bookingStatus.Booked,
                true,
                creationDate), cancellationToken);
        }
        else
        {
            Console.WriteLine("Количество людей не может быть отрицательным!");
        }
    }

    /// <summary>
    /// Отправка запроса отмены бронирования столика
    /// </summary>
    /// <param name="tableNumber">Номер столика</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task RequestBookingCancelEventBySmsAsync(int tableNumber, CancellationToken cancellationToken)
    {
        if (tableNumber > 0)
        {
            DateTime creationDate = DateTime.Now;
        
            _logger.Log(LogLevel.Information,$"Booking request to cancel table has been send to consumer at {creationDate}");
        
            await _bus.Publish<IBookingCancelRequestEvent>(new BookingCancelRequestEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                0,
                tableNumber,
                0,
                0,
                _bookingStatus.Canceled,
                true,
                creationDate), cancellationToken);
        }
        else
        {
            Console.WriteLine("Номер столика не может быть отрицательным!");
        }
    }

    /// <summary>
    /// Отправка запроса отмены бронирования столика
    /// </summary>
    /// <param name="tableNumber">Номер столика</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task RequestBookingCancelEventByPhoneAsync(int tableNumber, CancellationToken cancellationToken)
    {
        if (tableNumber > 0)
        {
            DateTime creationDate = DateTime.Now;
        
            _logger.Log(LogLevel.Information,$"Booking request to cancel table has been send to consumer at {creationDate}");
        
            await _bus.Publish<IBookingCancelRequestEvent>(new BookingCancelRequestEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                0,
                tableNumber,
                0,
                0,
                _bookingStatus.Canceled,
                true,
                creationDate), cancellationToken);
        }
        else
        {
            Console.WriteLine("Номер столика не может быть отрицательным!");
        }
    }
}