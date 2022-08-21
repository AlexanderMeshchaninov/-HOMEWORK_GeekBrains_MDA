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
            x.AddConsumer<RestaurantBookingRequestConsumer>(configurator =>
            {
                configurator.UseScheduledRedelivery(r =>
                {
                    r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(30));
                });
                configurator.UseMessageRetry(
                    r =>
                    {
                        r.Incremental(3, TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(2));
                    }
                );
            })
            .Endpoint(x =>
            {
                x.Temporary = true;
            });
            
            x.AddConsumer<RestaurantBookingRequestFaultConsumer>(configurator =>
            {
                configurator.UseScheduledRedelivery(r =>
                {
                    r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(30));
                });
                configurator.UseMessageRetry(
                    r =>
                    {
                        r.Incremental(3, TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(2));
                    }
                );
            })
            .Endpoint(x =>
            {
                x.Temporary = true;
            });

            x.AddConsumer<RestaurantBookingCancelRequestConsumer>(configurator =>
            {
                configurator.UseScheduledRedelivery(r =>
                {
                    r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(30));
                });
                configurator.UseMessageRetry(
                    r =>
                    {
                        r.Incremental(3, TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(2));
                    }
                );
            })
            .Endpoint(x =>
            {
                x.Temporary = true;
            });
            
            x.AddConsumer<RestaurantBookingCancelRequestFaultConsumer>(configurator =>
            {
                configurator.UseScheduledRedelivery(r =>
                {
                    r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(30));
                });
                configurator.UseMessageRetry(
                    r =>
                    {
                        r.Incremental(3, TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(2));
                    }
                );
            })
            .Endpoint(x =>
            {
                x.Temporary = true;
            });
            
            x.AddConsumer<RestaurantWaitingForAGuest>(configurator =>
            {
                configurator.UseScheduledRedelivery(r =>
                {
                    r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(30));
                });
                configurator.UseMessageRetry(
                    r =>
                    {
                        r.Incremental(3, TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(2));
                    }
                );
            })
            .Endpoint(x =>
            {
                x.Temporary = true;
            });
            
            x.AddConsumer<RestaurantWaitingForAGuestFault>(configurator =>
            {
                configurator.UseScheduledRedelivery(r =>
                {
                    r.Intervals(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(30));
                });
                configurator.UseMessageRetry(
                    r =>
                    {
                        r.Incremental(3, TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(2));
                    }
                );
            })
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
                
                // cfg.UseMessageRetry(r =>
                // {
                //     r.Exponential(5,
                //         TimeSpan.FromSeconds(1),
                //         TimeSpan.FromSeconds(100),
                //         TimeSpan.FromSeconds(5));
                //     r.Ignore<StackOverflowException>();
                //     r.Ignore<ArgumentNullException>(x => x.Message.Contains("Consumer"));
                // });
                            
                cfg.UseDelayedMessageScheduler();
                cfg.UseInMemoryOutbox();
                cfg.ConfigureEndpoints(ctx);
            });
                        
        });
        
        services.RegisterSingletonRestaurantServices(configuration, connectionSettings);
        services.AddHostedService<StartApp>();
    });
