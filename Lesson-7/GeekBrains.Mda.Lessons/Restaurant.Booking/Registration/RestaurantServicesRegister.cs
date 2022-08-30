using MassTransit.Audit;
using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Restaurant;
using Mda.Lessons.Core.RestaurantBooking;
using Restaurant.Booking.MassTransitSaga;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantBooking;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.AuditStore;
using Restaurant.Messaging.IdempotentConsumer;

namespace Restaurant.Booking.Registration;

public static class RestaurantServicesRegister
{
    public static IServiceCollection RegisterSingletonRestaurantServices(this IServiceCollection services,
        IConfiguration configuration,
        ConnectionSettings connectionSettings)
    {
        var bookingStatus = configuration
            .GetSection("RestaurantBookingStatus")
            .Get<RestaurantBookingStatus>();
        
        var menuStatus = configuration
            .GetSection("RestaurantMenuStatus")
            .Get<RestaurantMenuStatus>();

        var routingKeys = configuration
            .GetSection("RoutingKeys")
            .Get<string[]>();

        List<Table?> tables = new List<Table?>();
        
        for (int i = 1; i <= 10; i++)
        {
            tables.Add(new Table(i));
        }
        
        var phoneService = new RestaurantPhoneService(tables, bookingStatus);
        var smsService = new RestaurantSmsService(tables, bookingStatus);

        return services
            .AddSingleton(configuration)
            .AddSingleton(bookingStatus)
            .AddSingleton(menuStatus)
            .AddSingleton(phoneService)
            .AddSingleton(smsService)
            .AddSingleton<IMessageAuditStore, AuditStore>()
            .AddSingleton<IStartAppMenu, StartAppMenu>()
            .AddSingleton<IMakeIdempotentConsumer, MakeIdempotentConsumer>()
            .AddSingleton<RestaurantBookingStateMachine>()
            .AddSingleton<RestaurantBookingState>()
            .AddSingleton<IRestaurant, Restaurant.Restaurant>();
    }
}