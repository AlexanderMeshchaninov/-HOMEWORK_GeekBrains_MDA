using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Notifications.Consumers;
using Restaurant.Notifications.Notifiers;

namespace Restaurants.Tests;

[TestFixture]
public class NotificationConsumersTests
{
    private ServiceProvider _serviceProvider;
    private ITestHarness _testHarness;
    
    [OneTimeSetUp]
    public async Task Init()
    {
        
        _serviceProvider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<NotifyConsumer>();
                cfg.AddConsumerContainerTestHarness<NotifyConsumer>();
                
                cfg.AddConsumer<NotifyFaultConsumer>();
                cfg.AddConsumerContainerTestHarness<NotifyFaultConsumer>();
            })
            .AddLogging()
            .AddSingleton<Notifier>()
            .BuildServiceProvider(true);

        _testHarness = _serviceProvider.GetTestHarness();

        await _testHarness.Start();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _testHarness.OutputTimeline(TestContext.Out, options => options.Now().IncludeAddress());
        await _serviceProvider.DisposeAsync();
    }
    
    [Test]
    public async Task A_Any_notification_request_consumed()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();

        await _testHarness.Bus.Publish<INotifyEvent>( new NotifyEvent(
            orderId,
            clientId,
            "My super message for test!"));

        Assert.That(await _testHarness.Consumed.Any<INotifyEvent>());
    }
    
    [Test]
    public async Task A_Any_notification_request_fault_consumed()
    {
        var clientId = Guid.NewGuid();

        await _testHarness.Bus.Publish<INotifyEvent>( new NotifyEvent(
            Guid.Empty, 
            clientId,
            "My super message for test fault!"));

        Assert.That(await _testHarness.Consumed.Any<Fault<INotifyEvent>>());
    }
}