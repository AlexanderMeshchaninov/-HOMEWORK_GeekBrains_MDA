using MassTransit;
using Restaurant.Messaging.MessagesContracts;
using Restaurant.Notifications.Notifiers;

namespace Restaurant.Notifications.Consumers;

public class NotifierTableConsumer : IConsumer<ITableBooked>
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
     public Task Consume(ConsumeContext<ITableBooked> context)
     {
         bool result = context.Message.Success;
         
         _notifier.Accept(
             context.Message.OrderId, 
             result ? Accepted.Booking : Accepted.Rejected,
             context.Message.TableNumber,
             context.Message.Comments,
             context.Message.ClientId);
         
         return Task.CompletedTask;
     }
}