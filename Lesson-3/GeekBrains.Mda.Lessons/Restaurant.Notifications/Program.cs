using MassTransit;
using Mda.Lessons.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Notifications.Consumers;
using Restaurant.Notifications.Notifiers;

Console.OutputEncoding = System.Text.Encoding.UTF8;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) => 
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            IConfiguration configuration = hostContext.Configuration;
            ConnectionSettings connectionSettings = configuration
                .GetSection("RabbitMqConnectionSettings")
                .Get<ConnectionSettings>();
            
            services.AddMassTransit(x =>
            {
                x.AddConsumer<NotifierTableConsumer>();
                x.AddConsumer<NotifierKitchenConsumer>();

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
            services.AddSingleton<Notifier>();
        });