using MassTransit;
using MassTransit.Testing;
using Mda.Lessons.Core.Kitchen;
using Mda.Lessons.Core.Restaurant;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Kitchen.Consumer;
using Restaurant.Kitchen.Kitchen;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Messaging.IdempotentConsumer;

namespace Restaurants.Tests;

[TestFixture]
public class KitchenConsumersTests
{
    private ServiceProvider _serviceProvider;
    private ITestHarness _testHarness;
    
    [OneTimeSetUp]
    public async Task Init()
    {
        
        _serviceProvider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<KitchenBookingRequestConsumer>();
                cfg.AddConsumerContainerTestHarness<KitchenBookingRequestConsumer>();
                
                cfg.AddConsumer<KitchenBookingRequestFaultConsumer>();
                cfg.AddConsumerContainerTestHarness<KitchenBookingRequestFaultConsumer>();
                
                cfg.AddConsumer<KitchenBookingCancelRequestConsumer>();
                cfg.AddConsumerContainerTestHarness<KitchenBookingCancelRequestConsumer>();
                
                cfg.AddConsumer<KitchenBookingCancelRequestFaultConsumer>();
                cfg.AddConsumerContainerTestHarness<KitchenBookingCancelRequestFaultConsumer>();
                
                cfg.AddConsumer<KitchenInnerCancelConsumer>();
                cfg.AddConsumerContainerTestHarness<KitchenInnerCancelConsumer>();
                
                cfg.AddConsumer<KitchenInnerCancelFaultConsumer>();
                cfg.AddConsumerContainerTestHarness<KitchenInnerCancelFaultConsumer>();
            })
            .AddLogging()
            .AddSingleton<IKitchenManager, KitchenManager>()
            .AddSingleton<IMakeIdempotentConsumer, MakeIdempotentConsumer>()
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
    public async Task A_Any_kitchen_booking_request_consumed()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 2;
        
        await _testHarness.Bus.Publish<IBookingRequestEvent>( new BookingRequestEvent(
            orderId,
            clientId,
            Dish.Burger,
            0,
            0,
            countOfPersons,
            "Test booking request",
            true,
            DateTime.Now));

        Assert.That(await _testHarness.Consumed.Any<IBookingRequestEvent>());
    }
    
    [Test]
    public async Task booking_request_consumed_published_kitchen_ready()
    {
        var consumer = _serviceProvider.GetRequiredService <IConsumerTestHarness<KitchenBookingRequestConsumer>>();
        
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 2;
        
        await _testHarness.Bus.Publish<IBookingRequestEvent>( new BookingRequestEvent(
            orderId,
            clientId,
            Dish.None,
            0,
            0,
            countOfPersons,
            "Test booking request",
            true,
            DateTime.Now));
        
        Assert.That(consumer.Consumed.Select<IBookingRequestEvent>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);
        
        Assert.That(_testHarness.Published.Select<IKitchenReadyEvent>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);
    }
    
    [Test]
    public async Task A_Any_kitchen_booking_request_fault_consumed()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 2;
        
        await _testHarness.Bus.Publish<IBookingRequestEvent>( new BookingRequestEvent(
            orderId,
            clientId,
            Dish.Lasagna,
            0,
            0,
            countOfPersons,
            "Test booking request",
            true,
            DateTime.Now));

        Assert.That(await _testHarness.Consumed.Any<Fault<IBookingRequestEvent>>());
    }
    
    [Test]
    public async Task B_Any_kitchen_cancel_request_consumed()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 2;
        
        await _testHarness.Bus.Publish<IBookingRequestEvent>( new BookingRequestEvent(
            orderId,
            clientId,
            Dish.Burger,
            0,
            0,
            countOfPersons,
            "Test booking request",
            true,
            DateTime.Now));
        
        await _testHarness.Bus.Publish<IBookingCancelRequestEvent>( new BookingCancelRequestEvent(
            orderId,
            clientId,
            Dish.Burger,
            0,
            0,
            countOfPersons,
            "Test booking cancel request",
            true,
            DateTime.Now));

        Assert.That(await _testHarness.Consumed.Any<IBookingCancelRequestEvent>());
    }
    
    [Test]
    public async Task booking_request_consumed_published_kitchen_cancel()
    {
        var consumer = _serviceProvider.GetRequiredService<IConsumerTestHarness<KitchenBookingCancelRequestConsumer>>();
        
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 2;
        
        await _testHarness.Bus.Publish<IBookingCancelRequestEvent>( new BookingCancelRequestEvent(
            orderId,
            clientId,
            Dish.None,
            0,
            0,
            countOfPersons,
            "Test booking request",
            true,
            DateTime.Now));
        
        Assert.That(consumer.Consumed.Select<IBookingCancelRequestEvent>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);
        
        Assert.That(_testHarness.Published.Select<IKitchenCanceledEvent>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);
    }
    
    [Test]
    public async Task B_Any_kitchen_cancel_request_fault_consumed()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 2;
        
        await _testHarness.Bus.Publish<IBookingRequestEvent>( new BookingRequestEvent(
            orderId,
            clientId,
            Dish.Burger,
            0,
            0,
            countOfPersons,
            "Test booking request",
            true,
            DateTime.Now));
        
        await _testHarness.Bus.Publish<IBookingCancelRequestEvent>( new BookingCancelRequestEvent(
            Guid.Empty, 
            clientId,
            Dish.Burger,
            0,
            0,
            countOfPersons,
            "Test booking cancel fault request",
            true,
            DateTime.Now));

        Assert.That(await _testHarness.Consumed.Any<Fault<IBookingCancelRequestEvent>>());
    }
    
    [Test]
    public async Task C_Any_kitchen_cancel_inner_request_consumed()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 2;
        
        await _testHarness.Bus.Publish<IBookingRequestEvent>( new BookingRequestEvent(
            orderId,
            clientId,
            Dish.Burger,
            0,
            0,
            countOfPersons,
            "Test booking request",
            true,
            DateTime.Now));
        
        await _testHarness.Bus.Publish<IBookingInnerCancelEvent>( new BookingInnerCancelEvent(
            orderId, 
            clientId));

        Assert.That(await _testHarness.Consumed.Any<IBookingInnerCancelEvent>());
    }
    
    [Test]
    public async Task C_Any_kitchen_cancel_inner_request_fault_consumed()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 2;
        
        await _testHarness.Bus.Publish<IBookingRequestEvent>( new BookingRequestEvent(
            orderId,
            clientId,
            Dish.Burger,
            0,
            0,
            countOfPersons,
            "Test booking request",
            true,
            DateTime.Now));
        
        await _testHarness.Bus.Publish<IBookingInnerCancelEvent>( new BookingInnerCancelEvent(
            Guid.Empty, 
            clientId));

        Assert.That(await _testHarness.Consumed.Any<Fault<IBookingInnerCancelEvent>>());
    }
}