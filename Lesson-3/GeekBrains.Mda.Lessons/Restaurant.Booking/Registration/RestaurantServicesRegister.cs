using Mda.Lessons.Core.Restaurant;
using Mda.Lessons.Core.RestaurantServices;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantServices;

namespace Restaurant.Booking.Registration;

public static class RestaurantServicesRegister
{
    public static IServiceCollection RegisterSingletonRestaurantServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IPhoneService<Table>, ReceivePhoneFromClient>()
            .AddSingleton<ISmsService<Table>, ReceiveSmsFromClient>()
            .AddSingleton<INotificationService<Table>, RestaurantNotificationService>()
            .AddSingleton<IRestaurant, Restaurant.Restaurant>();
    }
}