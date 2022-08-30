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
            x.AddConsumer<KitchenBookingRequestConsumer>()
                .Endpoint(x =>
                {
                    x.Temporary = true;
                });
            
            x.AddConsumer<KitchenBookingCancelRequestConsumer>()
                .Endpoint(x => 
                {
                    x.Temporary = true;
                });
            
            x.AddConsumer<KitchenBookingRequestFaultConsumer>()
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