using MassTransit.Audit;
using Mda.Lessons.Core.Kitchen;
using Restaurant.Kitchen.Kitchen;
using Restaurant.Messaging.AuditStore;

namespace Restaurant.Kitchen.Registration;

public static class KitchenServicesRegister
{
    public static IServiceCollection RegisterSingletonKitchenServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IMessageAuditStore, AuditStore>()
            .AddSingleton<IKitchenManager, KitchenManager>();
    }
}