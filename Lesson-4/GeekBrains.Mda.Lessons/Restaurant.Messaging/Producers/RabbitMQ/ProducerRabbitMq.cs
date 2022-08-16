using System.Text;
using Mda.Lessons.Core.ApplicationSettings;
using Mda.Lessons.Core.Producers;
using RabbitMQ.Client;

namespace Restaurant.Messaging.Producers.RabbitMQ
{
    public class ProducerRabbitMq : IProducerRabbitMq
    {
        private readonly string _queueName;
        private readonly ConnectionFactory _connectionFactory;
        
        public ProducerRabbitMq(string queueName, ConnectionSettings connectionSettings)
        {
            _queueName = queueName;

            _connectionFactory = new ConnectionFactory()
            {
                UserName = connectionSettings.Login,
                Password = connectionSettings.Password,
                HostName = connectionSettings.HostName,
                Port = connectionSettings.Port,
                RequestedHeartbeat = TimeSpan.FromSeconds(10)
            };
        }
    
        /// <summary>
        /// Отправка напрямую в очередь
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public void SendToQueue(string message)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
        
            channel.QueueDeclare(
                queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: null,
                body: body);
        }
    
        /// <summary>
        /// Отправка Fanout (разветвление)
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public void SendToFanout(string message)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
        
            channel.ExchangeDeclare(
                exchange: _queueName, 
                type: ExchangeType.Fanout);
            
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: _queueName,
                routingKey: "",
                basicProperties: null,
                body: body);
        }
    
        /// <summary>
        /// Отправка Topic (посредством ключей)
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        /// <param name="routingKeyValue">название ключа</param>
        public void SendToTopic(string message, string routingKeyValue)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
        
            channel.ExchangeDeclare(
                exchange: _queueName, 
                type: "topic");
            
            string routingKey = (routingKeyValue.Length > 0) ? routingKeyValue : "anonymous.info";
                    
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: _queueName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
    }
}