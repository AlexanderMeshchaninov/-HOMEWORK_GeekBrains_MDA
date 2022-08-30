using Mda.Lessons.Dto;
using Restaurant.Booking.Restaurant;
using Restaurant.Messaging.Producers.MassTransit;
using State = Mda.Lessons.Core.Restaurant.State;

namespace Restaurant.Booking.RestaurantServices;

public sealed class RestaurantAutoCancelService
{
    private readonly IProducerMassTransit _restaurantProducerMassTransit;
    private readonly RestaurantBookingStatus _bookingStatus;
    public RestaurantAutoCancelService(
        List<Table> tables,
        int time,
        IProducerMassTransit restaurantProducerMassTransit,
        RestaurantBookingStatus bookingStatus)
    {
        _restaurantProducerMassTransit = restaurantProducerMassTransit;
        _bookingStatus = bookingStatus;
        
        _ = new Timer(async x => await Cancel(tables), tables, 0, time);
    }

    private async Task Cancel(List<Table> tables)
    {
        await Task.Run(async () =>
        {
            foreach (var table in tables)
            {
                if (table.State is State.Booked)
                {
                    await _restaurantProducerMassTransit
                        .PublishToKitchenTableCanceledAsync(table, _bookingStatus.AutoCanceled, new CancellationToken());

                    table.SetState(State.Free);
                }
            }
        });
    }
}