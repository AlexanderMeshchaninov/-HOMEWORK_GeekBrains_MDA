using MassTransit;
using Mda.Lessons.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Booking;
using Restaurant.Booking.Registration;
using Restaurant.Messaging.Consumers.RabbitMQ;
using Restaurant.Messaging.Producers.MassTransit;
using Restaurant.Messaging.Producers.RabbitMQ;

Console.OutputEncoding = System.Text.Encoding.UTF8;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        var bookingStatus = configuration
            .GetSection("RestaurantBookingStatus")
            .Get<RestaurantBookingStatus>();
        
        var menuStatus = configuration
            .GetSection("RestaurantMenuStatus")
            .Get<RestaurantMenuStatus>();
        
        var routingKeys = configuration
            .GetSection("RoutingKeys")
            .Get<string[]>();
        
        var connectionSettings = configuration
            .GetSection("RabbitMqConnectionSettings")
            .Get<ConnectionSettings>();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(connectionSettings.HostName, "/", x =>
                {
                    x.Username(connectionSettings.Login);
                    x.Password(connectionSettings.Password);
                });
                
                cfg.ConfigureEndpoints(ctx);
            });
        });

        var producerRabbitMq = new ProducerRabbitMq("topic", connectionSettings);
        services.AddSingleton(producerRabbitMq);
        
        var consumerRabbitMq = new ConsumerRabbitMq("topic", routingKeys, connectionSettings);
        services.AddSingleton(consumerRabbitMq);
        
        services.AddSingleton(configuration);
        services.AddSingleton(connectionSettings);
        services.AddSingleton(bookingStatus);
        services.AddSingleton(menuStatus);
        
        services.AddSingleton<IProducerMassTransit, ProducerMassTransit>();
        services.RegisterSingletonRestaurantServices();
        services.AddHostedService<StartApp>();
    });
