using Mda.Lessons.Core.RestaurantServices;
using Mda.Lessons.Dto;
using Microsoft.Extensions.Configuration;
using Restaurant.Booking.Restaurant;
using Restaurant.Messaging;

namespace Restaurant.Booking.RestaurantServices;

public sealed class RestaurantNotificationService : INotificationService<Table>
{
    private readonly Producer _producer;
    private readonly RoutingKeys _routingKeys;

    /// <summary>
    /// Класс отвечающий за публикацию сообщений в RabbitMq
    /// </summary>
    /// <param name="producer">Используется для отправки в RabbitMq</param>
    /// <param name="configuration">Конфигурация для RoutingKeys</param>
    public RestaurantNotificationService(
        Producer producer, 
        IConfiguration configuration)
    {
        _routingKeys = configuration
            .GetSection("RoutingKeys")
            .Get<RoutingKeys>();
        _producer = producer;
    }

    /// <summary>
    /// Отправка сообщения о бронировании через СМС
    /// </summary>
    /// <param name="bookedTable">Зарезирвированный столик</param>
    public async Task NotifyBySmsAsync(Table bookedTable)
    {
        await Task.Run(async () =>
        {
            await Task.Delay(5000);
            
            _producer.SendToTopic(bookedTable is null
                 ? "УВЕДОМЛЕНИЕ: К сожалению все столики заняты..."
                 : $"УВЕДОМЛЕНИЕ: Готово! Ваш столик номер: {bookedTable.Id}", 
                _routingKeys.Sms);
        });
    }

    /// <summary>
    /// Отправка сообщения о снятии брони через СМС
    /// </summary>
    /// <param name="bookedTable">Снятый с бронирования столик</param>
    public async Task NotifyCancelBySmsAsync(Table canceledTable)
    {
        await Task.Run(async () =>
        {
            await Task.Delay(5000);
            
            _producer.SendToTopic(canceledTable is null 
                ? "УВЕДОМЛЕНИЕ: Вы выбрали уже свободный столик!" 
                : $"УВЕДОМЛЕНИЕ: Столик номер {canceledTable?.Id}, теперь свободен!", 
                _routingKeys.Sms);
        });
    }
    
    /// <summary>
    /// Отправка сообщения о бронировании через Телефон
    /// </summary>
    /// <param name="bookedTable">Зарезирвированный столик</param>
    public async Task NotifyByPhoneAsync(Table bookedTable)
    {
        await Task.Run(async () =>
        {
            await Task.Delay(5000);
            
            _producer.SendToTopic(bookedTable is null
                ? "ЗВОНОК: К сожалению все столики заняты..."
                : $"ЗВОНОК: Готово! Ваш столик номер: {bookedTable.Id}", 
                _routingKeys.Phone);
        });
    }
    
    /// <summary>
    /// Отправка сообщения о снятии брони через Телефон
    /// </summary>
    /// <param name="bookedTable">Снятый с бронирования столик</param>
    public async Task NotifyCancelByPhoneAsync(Table canceledTable)
    {
        await Task.Run(async () =>
        {
            await Task.Delay(5000);
            
            _producer.SendToTopic(canceledTable is null 
                ? "УВЕДОМЛЕНИЕ: Вы выбрали уже свободный столик!" 
                : $"УВЕДОМЛЕНИЕ: Столик номер {canceledTable?.Id}, теперь свободен!", 
                _routingKeys.Phone);
        });
    }
}