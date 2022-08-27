using MassTransit;
using Mda.Lessons.Core.ApplicationSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Restaurant.Kitchen.Consumer;
using Restaurant.Kitchen.Registration;

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
            x.AddConsumer<KitchenBookingRequestConsumer>(configurator =>
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
                        r.Handle<ArgumentException>();
                    }
                );
            })
            .Endpoint(x =>
            {
                x.Temporary = true;
            });
            
            x.AddConsumer<KitchenBookingRequestFaultConsumer>(configurator =>
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
            
            x.AddConsumer<KitchenBookingCancelRequestConsumer>(configurator =>
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
            
            x.AddConsumer<KitchenBookingCancelRequestFaultConsumer>(configurator =>
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
            
            x.AddConsumer<KitchenInnerCancelConsumer>(configurator =>
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

            x.AddConsumer<KitchenInnerCancelFaultConsumer>(configurator =>
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

            x.AddDelayedMessageScheduler();

            x.UsingRabbitMq((ctx,cfg) =>
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
        
        services.RegisterSingletonKitchenServices(connectionSettings);
    });