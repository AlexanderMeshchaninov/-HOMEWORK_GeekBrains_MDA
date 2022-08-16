using MassTransit;

namespace Restaurant.Booking.MassTransitSaga;

public class RestaurantBookingState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public int CountOfPersons { get; set; }
    public int TableNumber { get; set; }
    public int Time { get; set; }
    public int ReadyEventStatus { get; set; }
    public Guid? ExpirationId { get; set; }
}