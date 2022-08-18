namespace Mda.Lessons.Core.Consumers;

public interface IConsumerRabbitMq<TDeliverEventArgs> where TDeliverEventArgs : class
{
     void ReceiveFromQueue(EventHandler<TDeliverEventArgs> receiveCallback);
     void ReceiveFromFanout(EventHandler<TDeliverEventArgs> receiveCallback);
     void ReceiveFromTopic(EventHandler<TDeliverEventArgs> receiveCallback);
}