using MassTransit;
using Restaurant.Messaging.Events;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Notifications.Notifiers;

namespace Restaurant.Notifications.Consumers;

public class NotifierTableConsumer : IConsumer<ITableBookedEvent>
{
     private readonly Notifier _notifier;
    
     public NotifierTableConsumer(Notifier notifier)
     {
         _notifier = notifier;
     }
    
     /// <summary>
     /// Метод получения сообщения от ресторана
     /// </summary>
     /// <param name="context">Контекст</param>
     /// <returns></returns>
     public Task Consume(ConsumeContext<ITableBookedEvent> context)
     {
         bool result = context.Message.Ready;
         
         _notifier.Accept(
             context.Message.OrderId, 
             result ? Accepted.Booking : Accepted.Rejected,
             0,
             context.Message.TableNumber,
             context.Message.Message,
             context.Message.ClientId);
         
         return Task.CompletedTask;
     }
}