using Mda.Lessons.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Notifications;

Console.OutputEncoding = System.Text.Encoding.UTF8;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) => 
    Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
    {
        IConfiguration configuration = context.Configuration;
        
        string[] routingKeys = configuration
            .GetSection("RoutingKeys")
            .Get<string[]>();
        RabbitMqConnectionSettings rabbitMqConnectionSettings = configuration
            .GetSection("RabbitMqConnectionSettings")
            .Get<RabbitMqConnectionSettings>();

        services.AddSingleton(routingKeys);
        services.AddSingleton(rabbitMqConnectionSettings);
        services.AddHostedService<Worker>();
    });