using System.Diagnostics;
using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Restaurant;
using Microsoft.Extensions.Hosting;

namespace Restaurant.Booking;

public class StartApp : BackgroundService
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

            if (choice is not 
                ((int) NotifyOptions.NotifyBySms or 
                (int) NotifyOptions.NotifyByPhone or
                (int) NotifyOptions.CancelBySms or
                (int) NotifyOptions.CancelByPhone))
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
                case (int) NotifyOptions.NotifyBySms:
                    await _restaurant.RequestBookingTableEventBySmsAsync(
                        InputValue(_menuStatus.BookingQuestion), DishChoiceMenu(), stoppingToken);
                    break;
                
                case (int) NotifyOptions.NotifyByPhone:
                    await _restaurant.RequestBookingTableEventByPhoneAsync(
                        InputValue(_menuStatus.BookingQuestion), DishChoiceMenu(), stoppingToken);
                    break;
                
                case (int) NotifyOptions.CancelBySms:
                    await _restaurant.RequestBookingCancelEventBySmsAsync(
                        InputValue(_menuStatus.CancelBookingQuestion), stoppingToken);
                    break;
                
                case (int) NotifyOptions.CancelByPhone:
                    await _restaurant.RequestBookingCancelEventByPhoneAsync(
                        InputValue(_menuStatus.CancelBookingQuestion), stoppingToken);
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

        static Dish DishChoiceMenu()
        {
            bool isWork = true;
            
            while (isWork)
            {
                Console.WriteLine();
                int dish = InputValue("Желаете выбрать предзаказ еды?" +
                                      $"\n1 - {Dish.None}" +
                                      $"\n2 - {Dish.Coffee}" +
                                      $"\n3 - {Dish.Burger}" +
                                      $"\n4 - {Dish.Pizza}" +
                                      $"\n5 - {Dish.Lasagna}");
                
                if (dish is not 
                    ((int) Dish.None or
                    (int) Dish.Coffee or
                    (int) Dish.Burger or
                    (int) Dish.Pizza or 
                    (int) Dish.Lasagna))
                {
                    Console.WriteLine("Выберите нужный вариант...");
                    continue;
                }
                
                isWork = false;
                
                switch (dish)
                {
                    case (int) Dish.None:
                        return Dish.None;
                    
                    case (int) Dish.Coffee:
                        return Dish.Coffee;
                    
                    case (int) Dish.Burger:
                        return Dish.Burger;
                    
                    case (int) Dish.Pizza:
                        return Dish.Pizza;
                    
                    case (int) Dish.Lasagna:
                        return Dish.Lasagna;
                }
            }
            
            return Dish.None;
        }
    }
}