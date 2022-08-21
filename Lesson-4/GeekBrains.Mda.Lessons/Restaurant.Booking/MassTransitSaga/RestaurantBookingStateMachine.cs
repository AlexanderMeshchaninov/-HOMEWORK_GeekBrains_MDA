using MassTransit;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;

namespace Restaurant.Booking.MassTransitSaga;

public class RestaurantBookingStateMachine : MassTransitStateMachine<RestaurantBookingState>
{
    public State AwaitingBookingApproved { get; private set; }
    public State AwaitingGuestArrived { get; private set; }
    public Event<IBookingRequestEvent> BookingRequestEvent { get; private set; }
    public Event<IBookingCancelRequestEvent> CancelBookingRequestEvent { get; private set; }
    public Event<ITableBookedEvent> TableBookedEvent { get; private set; }
    public Event<ITableCanceledEvent> TableCanceledEvent { get; private set; }
    public Event<IKitchenReadyEvent> KitchenReadyEvent { get; private set; }
    public Event<IKitchenCanceledEvent> KitchenCanceledEvent { get; private set; }
    public Event<IBookingApprovedEvent> BookingApprovedEvent { get; private set; }
    public Event<IGuestHasArrived> GuestHasArrived { get; private set; }
    public Event BookingApproved { get; private set;  }
    public Event BookingCanceledByClient { get; private set;  }
    public Event<Fault<IBookingRequestEvent>> BookingRequestFault { get; private set; }
    public Schedule<RestaurantBookingState, IBookingExpire> BookingExpired { get; private set; }
    public Schedule<RestaurantBookingState, IGuestArriveExpire> GuestExpired { get; private set; }

    public RestaurantBookingStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => BookingRequestEvent,
            x =>
                x.CorrelateById(context => context.Message.OrderId)
                    .SelectId(context => context.Message.OrderId));
        
        Event(() => CancelBookingRequestEvent,
            x =>
                x.CorrelateById(context => context.Message.OrderId)
                    .SelectId(context => context.Message.OrderId));
        
        Event(() => GuestHasArrived,
            x => 
                x.CorrelateById(context => context.Message.OrderId));

        Event(() => TableBookedEvent,
            x =>
                x.CorrelateById(context => context.Message.OrderId));

        Event(() => TableCanceledEvent,
            x =>
                x.CorrelateById(context => context.Message.OrderId));
        
        Event(() => KitchenReadyEvent,
            x =>
                x.CorrelateById(context => context.Message.OrderId));
        
        Event(() => KitchenCanceledEvent,
            x =>
                x.CorrelateById(context => context.Message.OrderId));
        
        Event(() => BookingApprovedEvent,
            x =>
                x.CorrelateById(context => context.Message.OrderId));
        
        Event(() => BookingRequestFault,
            x => 
                x.CorrelateById(context => context.Message.Message.OrderId));

        CompositeEvent(() => BookingApproved,
            x => x.ReadyEventStatus, TableBookedEvent, KitchenReadyEvent);

        CompositeEvent(() => BookingCanceledByClient,
            x => x.ReadyEventStatus, TableCanceledEvent, KitchenCanceledEvent);
        
        Schedule(() => BookingExpired,
        x => x.ExpirationId, x =>
        {
            x.Delay = TimeSpan.FromSeconds(10);
            x.Received = e => e.CorrelateById(context => context.Message.OrderId);
        });
        
        Schedule(() => GuestExpired,
        x => x.ExpirationId, x =>
        {
            x.Delay = TimeSpan.FromSeconds(13);
            x.Received = e => e.CorrelateById(context => context.Message.OrderId);
        });

        Initially
        (When(BookingRequestEvent)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.OrderId;
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.ClientId = context.Message.ClientId;
                    context.Saga.CountOfPersons = context.Message.CountOfPersons;
                })
                .Schedule(BookingExpired,
                    context => new BookingExpire (context.Saga),
                    context => TimeSpan.FromSeconds(10))
                .TransitionTo(AwaitingBookingApproved),
            
            When(CancelBookingRequestEvent)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.OrderId;
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.ClientId = context.Message.ClientId;
                    context.Saga.TableNumber = context.Message.TableNumber;
                })
                .TransitionTo(AwaitingBookingApproved),
            
            When(GuestHasArrived)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.OrderId;
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.Time = context.Message.Time;
                }).TransitionTo(AwaitingGuestArrived)
        );

        During(AwaitingBookingApproved,
            When(BookingApproved)
                .Unschedule(BookingExpired)
                .Publish(context =>
                    (INotifyEvent) new NotifyEvent(
                        context.Saga.OrderId,
                        context.Saga.ClientId,
                        context.Saga.TableNumber,
                        context.Saga.Time,
                        context.Saga.CountOfPersons,
                        $"[X] Стол на: {context.Saga.CountOfPersons} персон успешно забронирован на клиента " +
                        $"{context.Saga.ClientId}"))
                .Publish(context =>
                    (IBookingApprovedEvent) new BookingApprovedEvent(
                        context.Saga.OrderId))
                .TransitionTo(AwaitingGuestArrived),

            When(BookingRequestFault)
                .Then(context => Console.WriteLine("Ошибочка вышла!"))
                .Publish(context => (INotifyEvent) new NotifyEvent(
                    context.Saga.OrderId,
                    context.Saga.ClientId,
                    context.Saga.TableNumber,
                    context.Saga.Time,
                    context.Saga.CountOfPersons,
                    $"[!] Приносим извинения, стол забронировать не получилось по техническим причинам [!]"))
                .Publish(context => (IBookingCancellationEvent) 
                    new BookingCancellationEvent(context.Message.Message.OrderId))
                .Finalize(),

            When(BookingCanceledByClient)
                .Publish(context =>
                    (INotifyEvent) new NotifyEvent(
                        context.Saga.OrderId,
                        context.Saga.ClientId,
                        context.Saga.TableNumber,
                        context.Saga.Time,
                        context.Saga.CountOfPersons,
                        $"[!] Стол номер: {context.Saga.TableNumber} был успешно отменен по запросу клиента " +
                        $"{context.Saga.OrderId}"))
                .Finalize(),
            
            When(BookingExpired?.Received)
                .Publish(context =>
                    (INotifyEvent) new NotifyEvent(
                        context.Saga.OrderId,
                        context.Saga.ClientId,
                        context.Saga.TableNumber,
                        context.Saga.Time,
                        context.Saga.CountOfPersons,
                        $"[!] Отмена заказа {context.Saga.OrderId} по техническим причинам, приносим свои извинения"))
                .Finalize());
        
        During(AwaitingGuestArrived,
            When(GuestHasArrived)
                .Unschedule(GuestExpired)
                .Publish(context =>
                    (INotifyEvent) new NotifyEvent(
                        context.Saga.OrderId,
                        context.Saga.ClientId,
                        context.Saga.TableNumber,
                        context.Message.Time,
                        context.Saga.CountOfPersons,
                        $"[X] Гость {context.Saga.OrderId} прибыл в ресторан через {context.Message.Time} времени."))
                .Finalize(),
            
            When(BookingApprovedEvent)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.OrderId;
                    context.Saga.OrderId = context.Message.OrderId;
                })
                .Schedule(GuestExpired,
                    context => new GuestArriveExpire(context.Saga),
                    context => TimeSpan.FromSeconds(13))
                .TransitionTo(AwaitingGuestArrived),

            When(GuestExpired?.Received)
                .Publish(context =>
                    (INotifyEvent) new NotifyEvent(
                        context.Saga.OrderId,
                        context.Saga.ClientId,
                        context.Saga.TableNumber,
                        context.Saga.Time,
                        context.Saga.CountOfPersons,
                        $"[!] Отмена заказа {context.Saga.OrderId} гость не прибыл вовремя"))
                .Finalize());
                
        SetCompletedWhenFinalized();
    }
}