using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Restaurant;
using Mda.Lessons.Core.RestaurantBooking;

namespace Restaurant.Booking.RestaurantBooking;

public class StartAppMenu : IStartAppMenu
{
    private int _choice = 0;
    
    /// <summary>
    /// Меню выбора сервисов
    /// </summary>
    /// <param name="menuStatus">Класс со статусами для меню</param>
    /// <returns></returns>
    public int Menu(RestaurantMenuStatus menuStatus)
    {
        while (true)
        {
            Console.WriteLine();
            _choice = InputValue("Привет! Желаете забронировать столик?" +
                                                  $"\n1 - {menuStatus.NotifyBySms};" +
                                                  $"\n2 - {menuStatus.NotifyByPhone}" +
                                                  $"\n3 - {menuStatus.CancelBySms}" +
                                                  $"\n4 - {menuStatus.CancelByPhone}");

            if (_choice is not 
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
            
            break;
        }
        
        return _choice;
    }
    
    /// <summary>
    /// Меню выбора предзаказанных блюд
    /// </summary>
    /// <returns></returns>
    public Dish DishChoiceMenu()
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
    
    /// <summary>
    /// Метод обрабатывающий запросы используемый в классе StartApp для обеспечения работы меню
    /// </summary>
    /// <param name="text">Вводим необходимый текст</param>
    /// <returns>Получаем число</returns>
    public int InputValue(string text)
    {
        Console.WriteLine(text);
        int.TryParse(Console.ReadLine(), out int count);
        return count;
    }
}