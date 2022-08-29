using MassTransit;
using MassTransit.Audit;
using Mda.Lessons.Core.ApplicationSettings;
using Prometheus;
using Restaurant.Notifications.Consumers;
using Restaurant.Notifications.Registration;

namespace Restaurant.Notifications;

public class Startup
{
    public IConfiguration Configuration { get; }
        
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        
        IConfiguration configuration = Configuration;
            ConnectionSettings connectionSettings = configuration
                .GetSection("RabbitMqConnectionSettings")
                .Get<ConnectionSettings>();

            services.RegisterSingletonNotificationServices(connectionSettings);
            
            var serviceProvider = services.BuildServiceProvider();
            var auditStore = serviceProvider.GetService<IMessageAuditStore>();
            
            services.AddMassTransit(x =>
            {
                x.AddConsumer<NotifyConsumer>(configurator =>
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
                    
                    cfg.UsePrometheusMetrics(serviceName: "notification_service");
                    cfg.UseDelayedMessageScheduler();
                    cfg.UseInMemoryOutbox();
                    cfg.ConfigureEndpoints(ctx);
                    
                    cfg.ConnectSendAuditObservers(auditStore);
                    cfg.ConnectConsumeAuditObserver(auditStore);
                });
            });
            
            services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
                options.StopTimeout = TimeSpan.FromMinutes(1);
            });
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMetrics();
            endpoints.MapControllers();
        });
    }
}