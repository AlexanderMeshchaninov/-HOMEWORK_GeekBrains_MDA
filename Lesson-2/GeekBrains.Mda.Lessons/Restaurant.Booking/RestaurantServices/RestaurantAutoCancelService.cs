using Mda.Lessons.Dto;
using Microsoft.Extensions.Configuration;
using Restaurant.Booking.Restaurant;
using Restaurant.Messaging;

namespace Restaurant.Booking.RestaurantServices;
/// <summary>
/// Класс отвечающий за автоматическое снятие бронирования
/// </summary>
public sealed class RestaurantAutoCancelService
{
    private readonly RoutingKeys _routingKeys;
    private readonly Producer _producer;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tables">Данные о всех столах в ресторане</param>
    /// <param name="time">Заданное время через которое будет осуществляться проверка и снятие брони</param>
    /// <param name="producer">Используется для отправки в RabbitMQ</param>
    /// <param name="configuration">Конфигурация для RoutingKeys</param>
    public RestaurantAutoCancelService(
        List<Table> tables, 
        int time, 
        Producer producer,
        IConfiguration configuration)
    {
        _routingKeys = configuration
            .GetSection("RoutingKeys")
            .Get<RoutingKeys>();
        _producer = producer;
        
        Timer timer = new Timer(async x => await Cancel(tables), tables, 0, time);
    }

    private async Task Cancel(List<Table> tables)
    {
        await Task.Run(() =>
        {
            foreach (var table in tables)
            {
                if (table.State is State.Booked)
                {
                    _producer.SendToTopic(
                        message: $"- Столик номер {table.Id} освобожден! Бронь, к сожалению, слетела!", 
                        _routingKeys.Email);
                    
                    table.SetState(State.Free);
                }
            }
        });
    }
}