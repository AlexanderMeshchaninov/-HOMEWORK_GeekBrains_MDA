namespace Mda.Lessons.Core.Producers;

public interface IProducerRabbitMq
{
     void SendToQueue(string message);
     void SendToFanout(string message);
     void SendToTopic(string message, string routingKeyValue);
}