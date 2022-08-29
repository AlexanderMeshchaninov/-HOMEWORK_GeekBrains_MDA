using MassTransit;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.Consumers;

public class RestaurantWaitingForAGuestFaultConsumer : IConsumer<Fault<IBookingApprovedEvent>>
{
    private readonly ILogger<RestaurantWaitingForAGuestFaultConsumer> _logger;

    public RestaurantWaitingForAGuestFaultConsumer(ILogger<RestaurantWaitingForAGuestFaultConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<Fault<IBookingApprovedEvent>> context)
    {
        _logger.Log(LogLevel.Error, "Произошла ошибка при ожидании клиента!");
        await context.ConsumeCompleted;
    }
}