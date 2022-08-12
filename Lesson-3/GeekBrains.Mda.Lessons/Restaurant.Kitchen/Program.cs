using MassTransit;
using Mda.Lessons.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Kitchen.Consumer;
using Restaurant.Kitchen.Registration;
using Restaurant.Messaging.Producers.MassTransit;
using Restaurant.Notifications.Notifiers;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) => 
    Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
    {
        IConfiguration configuration = context.Configuration;
        ConnectionSettings connectionSettings = configuration
            .GetSection("RabbitMqConnectionSettings")
            .Get<ConnectionSettings>();
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TableBookedConsumer>();
            x.AddConsumer<TableCanceledConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(connectionSettings.HostName, "/", x =>
                {
                    x.Username(connectionSettings.Login);
                    x.Password(connectionSettings.Password);
                });
                    
                cfg.ConfigureEndpoints(context);
            });
        });
        
        
        services.AddSingleton(connectionSettings);
        services.RegisterSingletonKitchenServices();
        services.AddSingleton<Notifier>();
    });