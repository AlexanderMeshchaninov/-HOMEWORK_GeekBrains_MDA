namespace Mda.Lessons.Core.Producers;

public interface IKitchenProducerMassTransit
{
    Task PublishToNotificationsAsync(Guid orderId, 
        Guid clientId,
        int tableNumber,
        string comments, 
        bool success);
}