using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Restaurant;
using Mda.Lessons.Core.RestaurantBooking;

namespace Restaurant.Booking;

public class Worker : BackgroundService
{
    private readonly IRestaurant _restaurant;
    private readonly IStartAppMenu _startAppMenu;
    private readonly ILogger<Worker> _logger;
    private readonly RestaurantMenuStatus _menuStatus;
    public Worker(
        IRestaurant restaurant,
        IStartAppMenu startAppMenu,
        ILogger<Worker> logger,
        RestaurantMenuStatus menuStatus)
    {
        _restaurant = restaurant;
        _startAppMenu = startAppMenu;
        _logger = logger;
        _menuStatus = menuStatus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Log(LogLevel.Information,"StartApp is started...");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10_000, stoppingToken);
            
            switch (_startAppMenu.Menu(_menuStatus))
            {
                case (int) NotifyOptions.NotifyBySms:
                    _logger.Log(LogLevel.Information,"Request booking table");
                    await _restaurant.RequestBookingTableEventBySmsAsync(
                        _startAppMenu.InputValue(_menuStatus.BookingQuestion), 
                        _startAppMenu.DishChoiceMenu(), 
                        stoppingToken);
                    break;
                
                case (int) NotifyOptions.NotifyByPhone:
                    _logger.Log(LogLevel.Information,"Request booking table");
                    await _restaurant.RequestBookingTableEventByPhoneAsync(
                        _startAppMenu.InputValue(_menuStatus.BookingQuestion), 
                        _startAppMenu.DishChoiceMenu(), 
                        stoppingToken);
                    break;
                
                case (int) NotifyOptions.CancelBySms:
                    _logger.Log(LogLevel.Information,"Request booking cancel");
                    await _restaurant.RequestBookingCancelEventBySmsAsync(
                        _startAppMenu.InputValue(_menuStatus.CancelBookingQuestion), 
                        stoppingToken);
                    break;
                
                case (int) NotifyOptions.CancelByPhone:
                    _logger.Log(LogLevel.Information,"Request booking cancel");
                    await _restaurant.RequestBookingCancelEventByPhoneAsync(
                        _startAppMenu.InputValue(_menuStatus.CancelBookingQuestion), 
                        stoppingToken);
                    break;
            }

            _logger.Log(LogLevel.Information,"StartApp is stopped...");
            Console.WriteLine(_menuStatus.RestaurantGratitude);
        }
    }
}