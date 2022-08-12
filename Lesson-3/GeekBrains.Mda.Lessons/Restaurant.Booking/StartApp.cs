using System.Diagnostics;
using Mda.Lessons.Core.Restaurant;
using Mda.Lessons.Dto;
using Microsoft.Extensions.Hosting;

namespace Restaurant.Booking;

public sealed class StartApp : BackgroundService
{
    private readonly IRestaurant _restaurant;
    private readonly RestaurantMenuStatus _menuStatus;
    public StartApp(IRestaurant restaurant, RestaurantMenuStatus menuStatus)
    {
        _restaurant = restaurant;
        _menuStatus = menuStatus;
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
                    await _restaurant.BookingTableBySmsAsync(InputValue(_menuStatus.BookingQuestion), stoppingToken);
                    break;
                case 2:
                    await _restaurant.BookingTableByPhoneAsync(InputValue(_menuStatus.BookingQuestion), stoppingToken);
                    break;
                case 3:
                    await _restaurant.CancelBookingBySmsAsync(InputValue(_menuStatus.CancelBookingQuestion), stoppingToken);
                    break;
                case 4:
                    await _restaurant.CancelBookingByPhoneAsync(InputValue(_menuStatus.CancelBookingQuestion), stoppingToken);
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