using MassTransit.Audit;
using Mda.Lessons.Core.ApplicationSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Messaging.AuditStore;
using Restaurant.Notifications.Notifiers;

namespace Restaurant.Notifications.Registration;

public static class NotificationServicesRegister
{
    public static IServiceCollection RegisterSingletonNotificationServices(
        this IServiceCollection services, 
        ConnectionSettings connectionSettings)
    {
        return services
            .AddSingleton(connectionSettings)
            .AddSingleton<Notifier>()
            .AddSingleton<IMessageAuditStore, AuditStore>();
    }
}