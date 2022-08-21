using System.Data;
using MassTransit;
using Mda.Lessons.Core.ApplicationSettings;
using Restaurant.Booking.Restaurant;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Messaging.IdempotentConsumer;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequestEvent>
{
    private readonly RestaurantSmsService _smsService;
    private readonly RestaurantBookingStatus _bookingStatus;
    private readonly IMakeIdempotentConsumer _makeIdempotentConsumer;
    public RestaurantBookingRequestConsumer(
        RestaurantBookingStatus bookingStatus, 
        RestaurantSmsService smsService, 
        IMakeIdempotentConsumer makeIdempotentConsumer)
    {
        _bookingStatus = bookingStatus;
        _smsService = smsService;
        _makeIdempotentConsumer = makeIdempotentConsumer;
    }

    /// <summary>
    /// Подписка на событие бронирования столика в ресторане
    /// </summary>
    /// <param name="context">Контекст</param>
    public async Task Consume(ConsumeContext<IBookingRequestEvent> context)
    {
        try
        {
            await _makeIdempotentConsumer.MakeConsumerIdempotentInMemoryAsync(context);

            Table? bookedTable = await _smsService.BookTableBySmsFromRequestAsync(
                context.Message.OrderId,
                context.Message.CountOfPersons);

            if (bookedTable is not null)
            {
                Console.WriteLine(
                    $"[X] Столик номер [{bookedTable.TableNumber}] ЗАБРОНИРОВАН для клиента {context.Message.ClientId}");
                Console.WriteLine($"[X] Trying time: {DateTime.Now}");

                await context.Publish<ITableBookedEvent>(new TableBookedEvent(
                    context.Message.OrderId,
                    context.Message.ClientId,
                    context.Message.PreOrder,
                    bookedTable.TableNumber,
                    0,
                    context.Message.CountOfPersons,
                    _bookingStatus.Booked,
                    true,
                    context.Message.CreationDate));

                await context.ConsumeCompleted;
            }
            else
            {
                throw new ArgumentNullException("Столик пустой!");
            }
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"{ex}");
        }
        catch (DuplicateNameException ex)
        {
            Console.WriteLine($"{ex}");
        }
    }
}