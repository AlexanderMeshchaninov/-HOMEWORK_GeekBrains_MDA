using System.Text;
using Mda.Lessons.Dto;
using Microsoft.Extensions.Hosting;
using Restaurant.Messaging;

namespace Restaurant.Notifications;

public sealed class Worker : BackgroundService
{
    private readonly Consumer _consumer;
    public Worker(
        string[] routingKeys, 
        RabbitMqConnectionSettings connectionSettings)
    {
        #region BookingNotification
            //_consumer = new Consumer("BookingNotification");
        #endregion

        #region fanout
            //_consumer = new Consumer("fanout");
        #endregion
        
        _consumer = new Consumer(
            queueName: "topic_booking",
            routingKeys: routingKeys,
            connectionSettings: connectionSettings);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        #region ReceiveFromQueueAsync
            // await _consumer.ReceiveFromQueueAsync((sender, args) =>
            // {
            //     byte[] body = args.Body.ToArray();
            //
            //     string message = Encoding.UTF8.GetString(body);
            //
            //     Console.WriteLine(" [x] Received {0} ", message);
            //
            // });
        #endregion

        #region ReceiveFromFanoutAsync
            // await _consumer.ReceiveFromFanoutAsync((sender, args) =>
            // {
            //     byte[] body = args.Body.ToArray();
            //
            //     string message = Encoding.UTF8.GetString(body);
            //
            //     Console.WriteLine(" [x] Received {0} ", message);
            //
            // });
        #endregion
        
        _consumer.ReceiveFromTopic((sender, args) =>
        {
            byte[] body = args.Body.ToArray();
    
            string message = Encoding.UTF8.GetString(body);

            string routingKey = args.RoutingKey;
            
            Console.WriteLine(" [x] Received {0}, {1} ", message, routingKey);
        });

        return Task.CompletedTask;
    }
}