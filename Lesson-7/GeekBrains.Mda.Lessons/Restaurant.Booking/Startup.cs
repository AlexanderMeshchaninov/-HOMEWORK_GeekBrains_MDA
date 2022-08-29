using MassTransit;
using MassTransit.Audit;
using Mda.Lessons.Core.ApplicationSettings;
using Prometheus;
using Restaurant.Booking.Consumers;
using Restaurant.Booking.MassTransitSaga;
using Restaurant.Booking.Registration;

namespace Restaurant.Booking;

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
        
        var configuration = Configuration;
        var connectionSettings = configuration
             .GetSection("RabbitMqConnectionSettings")
             .Get<ConnectionSettings>();
         
        services.RegisterSingletonRestaurantServices(configuration, connectionSettings);
         
        var serviceProvider = services.BuildServiceProvider();
        var auditStore = serviceProvider.GetService<IMessageAuditStore>();
         
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
             
             x.AddConsumer<RestaurantWaitingForAGuestConsumer>(configurator =>
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
             
             x.AddConsumer<RestaurantInnerCancelConsumer>(configurator =>
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
             
             x.AddConsumer<RestaurantInnerCancelFaultConsumer>(configurator =>
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
                 
                 cfg.UsePrometheusMetrics(serviceName: "booking_service");
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
         
        services.AddHostedService<Worker>();
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