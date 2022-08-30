using System.Diagnostics;
using MassTransit;
using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Restaurant;
using Microsoft.Extensions.Hosting;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking;

public class StartApp : BackgroundService
{
    private readonly IRestaurant _restaurant;
    private readonly RestaurantMenuStatus _menuStatus;
    private readonly IBus _bus;
    public StartApp(IRestaurant restaurant, RestaurantMenuStatus menuStatus, IBus bus)
    {
        _restaurant = restaurant;
        _menuStatus = menuStatus;
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10_000, stoppingToken);

            Console.WriteLine();
            int choice = InputValue("Привет! Желаете забронировать столик?" +
                                    $"\n1 - {_menuStatus.NotifyBySms};" +
                                    $"\n2 - {_menuStatus.NotifyByPhone}" +
                                    $"\n3 - {_menuStatus.CancelBySms}" +
                                    $"\n4 - {_menuStatus.CancelByPhone}");
            
            if (choice is not (1 or 2 or 3 or 4))
            {
                Console.WriteLine("Введите пожалуйста варианты:" +
                                  "\n1 и 2 для заказа столика." +
                                  "\n3 и 4 для снятия брони.");
                continue;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            switch (choice)
            {
                case 1:
                    await _restaurant.RequestBookingTableEventBySmsAsync(InputValue(_menuStatus.BookingQuestion), stoppingToken);
                    break;
                case 2:
                    await _restaurant.RequestBookingTableEventByPhoneAsync(InputValue(_menuStatus.BookingQuestion), stoppingToken);
                    break;
                case 3:
                    await _restaurant.RequestBookingCancelEventBySmsAsync(InputValue(_menuStatus.CancelBookingQuestion), stoppingToken);
                    break;
                case 4:
                    await _restaurant.RequestBookingCancelEventByPhoneAsync(InputValue(_menuStatus.CancelBookingQuestion), stoppingToken);
                    break;
            }

            Console.WriteLine(_menuStatus.RestaurantGratitude);
            stopwatch.Stop();

            TimeSpan timeSpan = stopwatch.Elapsed;

            Console.WriteLine($"{timeSpan.Seconds:00}:{timeSpan.Milliseconds:00}");
        }

        static int InputValue(string text)
        {
            Console.WriteLine(text);
            int.TryParse(Console.ReadLine(), out int count);
            return count;
        }
    }
}