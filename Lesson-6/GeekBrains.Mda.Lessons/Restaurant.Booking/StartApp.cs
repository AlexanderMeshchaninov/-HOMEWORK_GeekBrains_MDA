using System.Diagnostics;
using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Restaurant;
using Mda.Lessons.Core.RestaurantBooking;
using Microsoft.Extensions.Hosting;

namespace Restaurant.Booking;

public class StartApp : BackgroundService
{
    private readonly IRestaurant _restaurant;
    private readonly IStartAppMenu _startAppMenu;
    private readonly RestaurantMenuStatus _menuStatus;
    public StartApp(
        IRestaurant restaurant,
        IStartAppMenu startAppMenu,
        RestaurantMenuStatus menuStatus)
    {
        _restaurant = restaurant;
        _startAppMenu = startAppMenu;
        _menuStatus = menuStatus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10_000, stoppingToken);
            
            switch (_startAppMenu.Menu(_menuStatus))
            {
                case (int) NotifyOptions.NotifyBySms:
                    await _restaurant.RequestBookingTableEventBySmsAsync(
                        _startAppMenu.InputValue(_menuStatus.BookingQuestion), 
                        _startAppMenu.DishChoiceMenu(), 
                        stoppingToken);
                    break;
                
                case (int) NotifyOptions.NotifyByPhone:
                    await _restaurant.RequestBookingTableEventByPhoneAsync(
                        _startAppMenu.InputValue(_menuStatus.BookingQuestion), 
                        _startAppMenu.DishChoiceMenu(), 
                        stoppingToken);
                    break;
                
                case (int) NotifyOptions.CancelBySms:
                    await _restaurant.RequestBookingCancelEventBySmsAsync(
                        _startAppMenu.InputValue(_menuStatus.CancelBookingQuestion), 
                        stoppingToken);
                    break;
                
                case (int) NotifyOptions.CancelByPhone:
                    await _restaurant.RequestBookingCancelEventByPhoneAsync(
                        _startAppMenu.InputValue(_menuStatus.CancelBookingQuestion), 
                        stoppingToken);
                    break;
            }

            Console.WriteLine(_menuStatus.RestaurantGratitude);
        }
    }
}