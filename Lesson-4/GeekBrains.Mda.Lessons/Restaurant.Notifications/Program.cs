using MassTransit;
using Mda.Lessons.Core.ApplicationSettings;
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
                x.AddConsumer<NotifyConsumer>()
                    .Endpoint(x => 
                    {
                        x.Temporary = true;
                    });

                x.AddDelayedMessageScheduler();
                
                x.UsingRabbitMq((context,cfg) =>
                {
                    cfg.Host(connectionSettings.HostName, "/", x =>
                    {
                        x.Username(connectionSettings.Login);
                        x.Password(connectionSettings.Password);
                    });
                            
                    cfg.UseMessageRetry(r =>
                    {
                        r.Exponential(5,
                            TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(100),
                            TimeSpan.FromSeconds(5));
                        r.Ignore<StackOverflowException>();
                        r.Ignore<ArgumentNullException>(x => x.Message.Contains("Consumer"));
                    });
                    
                    cfg.UseDelayedMessageScheduler();
                    cfg.UseInMemoryOutbox();
                    cfg.ConfigureEndpoints(context);
                });
            });
            
            services.AddSingleton(connectionSettings);
            services.AddSingleton<NewNotifier>();
        });