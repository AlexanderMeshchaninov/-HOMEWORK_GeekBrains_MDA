using MassTransit;
using MassTransit.Testing;
using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Kitchen;
using Mda.Lessons.Core.Restaurant;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Restaurant.Booking.Consumers;
using Restaurant.Booking.MassTransitSaga;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Kitchen.Consumer;
using Restaurant.Kitchen.Kitchen;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Messaging.IdempotentConsumer;
using Restaurant.Notifications.Consumers;
using Restaurant.Notifications.Notifiers;

namespace Restaurants.Tests;

[TestFixture]
public class SagaTests
{
    private ServiceProvider _serviceProvider;
    private InMemoryTestHarness _testHarness;
    
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
            .AddMassTransitInMemoryTestHarness(cfg =>
            {
                cfg.AddConsumer<RestaurantBookingRequestConsumer>();

                cfg.AddConsumer<RestaurantWaitingForAGuestConsumer>();

                cfg.AddConsumer<KitchenBookingRequestConsumer>();

                cfg.AddConsumer<NotifyConsumer>();

                cfg.AddSagaStateMachine<RestaurantBookingStateMachine, RestaurantBookingState>()
                    .InMemoryRepository();
                    
                cfg.AddSagaStateMachineTestHarness<RestaurantBookingStateMachine, RestaurantBookingState>();
                cfg.AddDelayedMessageScheduler();
            })
            .AddLogging()
            .AddSingleton<RestaurantSmsService>()
            .AddSingleton<IKitchenManager, KitchenManager>()
            .AddSingleton<Notifier>()
            .AddSingleton<IMakeIdempotentConsumer, MakeIdempotentConsumer>()
            .AddSingleton(bookingStatus)
            .BuildServiceProvider(true);
        
        _testHarness = _serviceProvider.GetRequiredService<InMemoryTestHarness>();

        _testHarness.OnConfigureInMemoryBus += configurator => configurator.UseDelayedMessageScheduler();
        
        await _testHarness.Start();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _testHarness.Stop();
        await _serviceProvider.DisposeAsync();
    }
    
    [Test]
    public async Task Should_be_easy()
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
        
        Assert.That(await _testHarness.Published.Any<IBookingRequestEvent>());
        Assert.That(await _testHarness.Consumed.Any<IBookingRequestEvent>());
        Assert.That(await _testHarness.Consumed.Any<IBookingApprovedEvent>());
        
        var sagaHarness = _serviceProvider
            .GetRequiredService<ISagaStateMachineTestHarness<RestaurantBookingStateMachine, RestaurantBookingState>>();

        Assert.That(await sagaHarness.Consumed.Any<IBookingRequestEvent>());
        Assert.That(await sagaHarness.Created.Any(x => x.CorrelationId == orderId));

        Assert.That(await sagaHarness.Consumed.Any<IBookingApprovedEvent>());
        Assert.That(await sagaHarness.Created.Any(x => x.CorrelationId == orderId));
        
        var saga = sagaHarness.Created.Contains(orderId);

        Assert.That(saga, Is.Not.Null);
        Assert.That(saga.ClientId, Is.EqualTo(clientId));
        Assert.That(await _testHarness.Published.Any<ITableBookedEvent>());
        Assert.That(await _testHarness.Published.Any<IKitchenReadyEvent>());
        Assert.That(await _testHarness.Published.Any<IGuestHasArrivedEvent>());
        Assert.That(await _testHarness.Published.Any<INotifyEvent>());
    }
}