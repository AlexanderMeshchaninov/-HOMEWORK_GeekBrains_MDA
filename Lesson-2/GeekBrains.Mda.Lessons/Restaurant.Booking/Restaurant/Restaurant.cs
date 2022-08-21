using Mda.Lessons.Core;
using Mda.Lessons.Core.RestaurantServices;
using Mda.Lessons.Dto;
using Microsoft.Extensions.Configuration;
using Restaurant.Booking.RestaurantServices;
using Restaurant.Messaging;

namespace Restaurant.Booking.Restaurant;

/// <summary>
/// Ресторан
/// </summary>
public sealed class Restaurant : IRestaurant
{
    private readonly IPhoneService<Table> _restaurantPhoneService;
    private readonly ISmsService<Table> _restaurantSmsService;
    private readonly INotificationService<Table> _notificationService;
    private CancellationTokenSource _cts;
    
    public Restaurant(IConfiguration configuration)
    {
        RabbitMqConnectionSettings connectionSettings = configuration
            .GetSection("RabbitMqConnectionSettings")
            .Get<RabbitMqConnectionSettings>();
        
        List<Table> tables = new List<Table>();
        for (int i = 1; i <= 10; i++)
        {
            tables.Add(new Table(i));
        }
        
        Producer producer = new Producer(
            queueName: "topic_booking",
            connectionSettings: connectionSettings);
        
        RestaurantAutoCancelService restaurantAutoCancelService = new RestaurantAutoCancelService(
            tables: tables,
            time: 100_000, 
            producer: producer,
            configuration: configuration);
        
        //Поскольку пока DI не реализован
        _restaurantPhoneService = new RestaurantPhoneService(tables);
        _restaurantSmsService = new RestaurantSmsService(tables);
        _notificationService = new RestaurantNotificationService(producer, configuration);
    }
    
    /// <summary>
    /// Бронирование столика через СМС
    /// </summary>
    /// <param name="countOfPersons">Количество запрашиваемых персон</param>
    public async Task BookingTableBySmsAsync(int countOfPersons)
    {
        Table bookedTable = await _restaurantSmsService.BookFreeTableBySmsAsync(countOfPersons);
        await _notificationService.NotifyBySmsAsync(bookedTable);
    }

    /// <summary>
    /// Бронирование столика через Телефон
    /// </summary>
    /// <param name="countOfPersons">Количество запрашиваемых персон</param>
    public async Task BookingTableByPhoneAsync(int countOfPersons)
    {
        Table bookedTable = _restaurantPhoneService.BookFreeTableByPhone(countOfPersons);
        await _notificationService.NotifyByPhoneAsync(bookedTable);
    }

    /// <summary>
    /// Снятие брони через СМС
    /// </summary>
    /// <param name="tableId">Номер столика для снятия брони</param>
    public async Task CancelBookingBySmsAsync(int tableId)
    {
        Table canceledTable = await _restaurantSmsService.CancelBookingBySmsAsync(tableId);
        await _notificationService.NotifyCancelBySmsAsync(canceledTable);
    }

    /// <summary>
    /// Снятие брони через Телефон
    /// </summary>
    /// <param name="tableId">Номер столика для снятия брони</param>
    public async Task CancelBookingByPhoneAsync(int tableId)
    {
        Table canceledTable = _restaurantPhoneService.CancelBookingByPhone(tableId);
        await _notificationService.NotifyCancelByPhoneAsync(canceledTable);
    }
}