using MassTransit;
using MassTransit.Testing;
using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Restaurant;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Booking.Consumers;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Messaging.IdempotentConsumer;

namespace Restaurants.Tests;

[TestFixture]
public class RestaurantConsumersTests
{
    private ServiceProvider _serviceProvider;
    private ITestHarness _testHarness;
    
    [OneTimeSetUp]
    public async Task Init()
    {
        var bookingStatus = new RestaurantBookingStatus()
        {
            Booked = "Клиент забронировал столик",
            Canceled = "Клиент cнял бронь столика",
            AutoCanceled = "Столик освобожден! Бронь, к сожалению, слетела!",
            AlreadyExists = "Столик уже свободен!",
            CancelTableGreeting = "Добрый день! Введите номер заказанного столика, чтобы снять бронь",
            BookTableSmsGreeting = "Добрый день! Подождите секунду я подберу столик и подтвержу вашу бронь, вам придет уведомление...",
            BookTablePhoneGreeting = "Добрый день! Подождите секунду я подберу столик и подтвержу вашу бронь, оставайтесь на линии..."
        };
        
        _serviceProvider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<RestaurantBookingRequestConsumer>();
                cfg.AddConsumerContainerTestHarness<RestaurantBookingRequestConsumer>();
                
                cfg.AddConsumer<RestaurantBookingRequestFaultConsumer>();
                cfg.AddConsumerContainerTestHarness<RestaurantBookingRequestFaultConsumer>();
                
                cfg.AddConsumer<RestaurantBookingCancelRequestConsumer>();
                cfg.AddConsumerContainerTestHarness<RestaurantBookingCancelRequestConsumer>();
                
                cfg.AddConsumer<RestaurantBookingCancelRequestFaultConsumer>();
                cfg.AddConsumerContainerTestHarness<RestaurantBookingCancelRequestFaultConsumer>();
                
                cfg.AddConsumer<RestaurantInnerCancelConsumer>();
                cfg.AddConsumerContainerTestHarness<RestaurantInnerCancelConsumer>();
                
                cfg.AddConsumer<RestaurantInnerCancelFaultConsumer>();
                cfg.AddConsumerContainerTestHarness<RestaurantInnerCancelFaultConsumer>();
                
                cfg.AddConsumer<RestaurantWaitingForAGuestConsumer>();
                cfg.AddConsumerContainerTestHarness<RestaurantWaitingForAGuestConsumer>();
                
                cfg.AddConsumer<RestaurantWaitingForAGuestFaultConsumer>();
                cfg.AddConsumerContainerTestHarness<RestaurantWaitingForAGuestFaultConsumer>();
            })
            .AddLogging()
            .AddSingleton<RestaurantSmsService>()
            .AddSingleton<IMakeIdempotentConsumer, MakeIdempotentConsumer>()
            .AddSingleton(bookingStatus)
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
    public async Task A_Any_booking_request_consumed()
    {
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

        Assert.That(await _testHarness.Consumed.Any<IBookingRequestEvent>());
        Assert.That(await _testHarness.Published.Any<IBookingRequestEvent>());
    }
    
    [Test]
    public async Task booking_request_consumed_published_table_booked()
    {
        var consumer = _serviceProvider.GetRequiredService <IConsumerTestHarness<RestaurantBookingRequestConsumer>>();
        
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
        
        Assert.That(_testHarness.Published.Select<ITableBookedEvent>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);
    }
    
    [Test]
    public async Task A_Any_booking_request_fault_consumed()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 1_000;
        
        await _testHarness.Bus.Publish<IBookingRequestEvent>( new BookingRequestEvent(
            orderId,
            clientId,
            Dish.None,
            0,
            0,
            countOfPersons,
            "Test booking fault request",
            true,
            DateTime.Now));
        
        Assert.That(await _testHarness.Consumed.Any<Fault<IBookingRequestEvent>>());
    }
    
    [Test]
    public async Task B_Any_booking_cancel_request_consumed()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 2;
        int tableNumber = 2;
        
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
        
        await _testHarness.Bus.Publish<IBookingCancelRequestEvent>( new BookingCancelRequestEvent(
            orderId,
            clientId,
            Dish.None,
            tableNumber,
            0,
            0,
            "Test booking cancel request",
            true,
            DateTime.Now));

        Assert.That(await _testHarness.Consumed.Any<IBookingCancelRequestEvent>());
        Assert.That(await _testHarness.Published.Any<IBookingCancelRequestEvent>());
    }
    
    [Test]
    public async Task booking_cancel_request_consumed_published_table_canceled()
    {
        var consumer = _serviceProvider.GetRequiredService<IConsumerTestHarness<RestaurantBookingCancelRequestConsumer>>();
        
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 2;
        int tableNumber = 1;
        
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
        
        await _testHarness.Bus.Publish<IBookingCancelRequestEvent>( new BookingCancelRequestEvent(
            orderId,
            clientId,
            Dish.None,
            tableNumber,
            0,
            0,
            "Test booking cancel request",
            true,
            DateTime.Now));
        
        Assert.That(consumer.Consumed.Select<IBookingCancelRequestEvent>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);
        
        Assert.That(_testHarness.Published.Select<ITableCanceledEvent>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);
    }
    
    [Test]
    public async Task B_Any_booking_cancel_request_fault_consumed()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int tableNumber = 1_000;
        
        await _testHarness.Bus.Publish<IBookingCancelRequestEvent>( new BookingCancelRequestEvent(
            orderId,
            clientId,
            Dish.None,
            tableNumber,
            0,
            0,
            "Test booking cancel fault request",
            true,
            DateTime.Now));

        Assert.That(await _testHarness.Consumed.Any<Fault<IBookingCancelRequestEvent>>());
    }
    
    [Test]
    public async Task C_Any_booking_cancel_inner_request_consumed()
    {
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

        await _testHarness.Bus.Publish<IBookingInnerCancelEvent>( new BookingInnerCancelEvent(
            orderId,
            clientId));

        Assert.That(await _testHarness.Consumed.Any<IBookingCancelRequestEvent>());
        Assert.That(await _testHarness.Published.Any<IBookingCancelRequestEvent>());
        
    }
    
    [Test]
    public async Task C_Any_booking_cancel_inner_request_fault_consumed()
    {
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 3;
        
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

        await _testHarness.Bus.Publish<IBookingInnerCancelEvent>( new BookingInnerCancelEvent(
            Guid.Empty, 
            Guid.Empty));

        Assert.That(await _testHarness.Consumed.Any<Fault<IBookingCancelRequestEvent>>());
    }
    
    [Test]
    public async Task D_Any_waiting_for_a_guest_consumed()
    {
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
        
        await _testHarness.Bus.Publish<IBookingApprovedEvent>( new BookingApprovedEvent(
            orderId,
            clientId));

        Assert.That(await _testHarness.Consumed.Any<IBookingApprovedEvent>());
        Assert.That(await _testHarness.Published.Any<IBookingApprovedEvent>());
    }
    
    [Test]
    public async Task D_Any_waiting_for_a_guest_fault_consumed()
    {
        var clientId = Guid.NewGuid();
        
        await _testHarness.Bus.Publish<IBookingApprovedEvent>( new BookingApprovedEvent(
            Guid.Empty,
            clientId));

        Assert.That(await _testHarness.Consumed.Any<Fault<IBookingApprovedEvent>>());
    }
    
    [Test]
    public async Task booking_approved_request_consumed_published_guest_has_arrived()
    {
        var consumer = _serviceProvider.GetRequiredService<IConsumerTestHarness<RestaurantWaitingForAGuestConsumer>>();
        
        var orderId = Guid.NewGuid();
        var clientId = Guid.NewGuid();
        int countOfPersons = 2;
        int tableNumber = 1;
        
        await _testHarness.Bus.Publish<IBookingApprovedEvent>( new BookingApprovedEvent(
            orderId,
            clientId));

        Assert.That(consumer.Consumed.Select<IBookingApprovedEvent>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);
        
        Assert.That(_testHarness.Published.Select<IGuestHasArrivedEvent>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);
    }
}