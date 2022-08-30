using Mda.Lessons.Core.Consumers;
using Mda.Lessons.Dto;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Restaurant.Messaging.Consumers.RabbitMQ;

public sealed class ConsumerRabbitMq : IConsumerRabbitMq<BasicDeliverEventArgs>
{
    private readonly string[] _routingKeys;
    private readonly string _queueName;
    private readonly IModel _channel;

    public ConsumerRabbitMq(
        string queueName,
        string[] routingKeys,
        ConnectionSettings connectionSettings)
    {
        _queueName = queueName;
        _routingKeys = routingKeys;

        ConnectionFactory factory = new ConnectionFactory()
        {
            UserName = connectionSettings.Login,
            Password = connectionSettings.Password,
            HostName = connectionSettings.HostName,
            Port = connectionSettings.Port,
            RequestedHeartbeat = TimeSpan.FromSeconds(10)
        };

        IConnection connection = factory.CreateConnection();
        _channel = connection.CreateModel();
    }

    /// <summary>
    /// Метод получения сообщения из очереди RabbitMq
    /// </summary>
    /// <param name="receiveCallback"></param>
    public void ReceiveFromQueue(EventHandler<BasicDeliverEventArgs> receiveCallback)
    {
        _channel.QueueDeclare(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
        consumer.Received += receiveCallback;

        _channel.BasicConsume(
            queue: _queueName,
            autoAck: true,
            consumer: consumer);
    }

    /// <summary>
    /// Метод получения сообщения из Fanout RabbitMq
    /// </summary>
    /// <param name="receiveCallback"></param>
    public void ReceiveFromFanout(EventHandler<BasicDeliverEventArgs> receiveCallback)
    {
        _channel.ExchangeDeclare(
            exchange: _queueName,
            type: ExchangeType.Fanout);

        var queueName = _channel.QueueDeclare().QueueName;

        _channel.QueueBind(
            queue: queueName,
            exchange: _queueName,
            routingKey: "");

        EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
        consumer.Received += receiveCallback;

        _channel.BasicConsume(
            queue: queueName,
            autoAck: true,
            consumer: consumer);
    }

    /// <summary>
    /// Метод получения сообщения из Topic RabbitMq
    /// </summary>
    /// <param name="receiveCallback"></param>
    public void ReceiveFromTopic(EventHandler<BasicDeliverEventArgs> receiveCallback)
    {
        _channel.ExchangeDeclare(
            exchange: _queueName,
            type: "topic");

        var queueName = _channel.QueueDeclare().QueueName;

        foreach (var bindingKey in _routingKeys)
        {
            _channel.QueueBind(
                queue: queueName,
                exchange: _queueName,
                routingKey: bindingKey);
        }

        EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
        consumer.Received += receiveCallback;

        _channel.BasicConsume(
            queue: queueName,
            autoAck: true,
            consumer: consumer);
    }
}