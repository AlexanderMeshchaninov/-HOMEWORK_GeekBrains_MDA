using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Restaurant;

namespace Mda.Lessons.Core.RestaurantBooking;

public interface IStartAppMenu
{
    int Menu(RestaurantMenuStatus menuStatus);
    Dish DishChoiceMenu();
    int InputValue(string text);
}