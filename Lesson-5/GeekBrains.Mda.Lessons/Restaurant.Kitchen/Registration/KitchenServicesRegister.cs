using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Kitchen;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Kitchen.Kitchen;
using Restaurant.Messaging.Producers.MassTransit;

namespace Restaurant.Kitchen.Registration;

public static class KitchenServicesRegister
{
    public static IServiceCollection RegisterSingletonKitchenServices(
        this IServiceCollection services, 
        ConnectionSettings connectionSettings)
    {
        return services
            .AddSingleton(connectionSettings)
            .AddTransient<IProducerMassTransit, ProducerMassTransit>()
            .AddSingleton<IKitchenManager, KitchenManager>();
    }
}