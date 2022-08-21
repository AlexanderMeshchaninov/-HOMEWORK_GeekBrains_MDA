using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Restaurant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Booking.MassTransitSaga;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.Consumers.RabbitMQ;
using Restaurant.Messaging.Producers.MassTransit;
using Restaurant.Messaging.Producers.RabbitMQ;

namespace Restaurant.Booking.Registration;

public static class RestaurantServicesRegister
{
    public static IServiceCollection RegisterSingletonRestaurantServices(
        this IServiceCollection services,
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

        List<Table> tables = new List<Table>();
        
        for (int i = 1; i <= 10; i++)
        {
            tables.Add(new Table(i));
        }
        
        var producerRabbitMq = new ProducerRabbitMq("topic", connectionSettings);
        var consumerRabbitMq = new ConsumerRabbitMq("topic", routingKeys, connectionSettings);
        
        var phoneService = new RestaurantPhoneService(tables, bookingStatus);
        var smsService = new RestaurantSmsService(tables, bookingStatus);
        
        return services
            .AddSingleton(producerRabbitMq)
            .AddSingleton(configuration)
            .AddSingleton(connectionSettings)
            .AddSingleton(bookingStatus)
            .AddSingleton(menuStatus)
            .AddSingleton(phoneService)
            .AddSingleton(smsService)
            .AddSingleton(consumerRabbitMq)
            .AddSingleton<RestaurantBookingStateMachine>()
            .AddSingleton<RestaurantBookingState>()
            .AddSingleton<IRestaurant, Restaurant.Restaurant>()
            .AddSingleton<IRestaurantNotificationService, RestaurantNotificationService>()
            .AddSingleton<IProducerMassTransit, ProducerMassTransit>();
    }
}