using MassTransit;
using Mda.Lessons.Core.ApplicationSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Booking;
using Restaurant.Booking.Consumers;
using Restaurant.Booking.MassTransitSaga;
using Restaurant.Booking.Registration;

Console.OutputEncoding = System.Text.Encoding.UTF8;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;
        var connectionSettings = configuration
            .GetSection("RabbitMqConnectionSettings")
            .Get<ConnectionSettings>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<RestaurantBookingRequestConsumer>()
                .Endpoint(x =>
                {
                    x.Temporary = true;
                });

            x.AddConsumer<RestaurantBookingCancelRequestConsumer>()
                .Endpoint(x =>
                {
                    x.Temporary = true;
                });
            
            x.AddConsumer<RestaurantBookingRequestFaultConsumer>()
                .Endpoint(x =>
                {
                    x.Temporary = true;
                });
            
            x.AddConsumer<RestaurantWaitingForAGuest>()
                .Endpoint(x =>
                {
                    x.Temporary = true;
                });
            
            x.AddSagaStateMachine<RestaurantBookingStateMachine, RestaurantBookingState>()
                .Endpoint(x => 
                {
                    x.Temporary = true;
                })
                .InMemoryRepository();

            x.AddDelayedMessageScheduler();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(connectionSettings.HostName, "/", x =>
                {
                    x.Username(connectionSettings.Login);
                    x.Password(connectionSettings.Password);
                });
                            
                cfg.UseDelayedMessageScheduler();
                cfg.UseInMemoryOutbox();
                cfg.ConfigureEndpoints(ctx);
            });
                        
        });
        
        services.RegisterSingletonRestaurantServices(configuration, connectionSettings);
        services.AddHostedService<StartApp>();
    });
