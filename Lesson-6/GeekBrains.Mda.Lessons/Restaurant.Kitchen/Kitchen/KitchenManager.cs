using Mda.Lessons.Core.Kitchen;
using Restaurant.Messaging.Producers.MassTransit;

namespace Restaurant.Kitchen.Kitchen;

public class KitchenManager : IKitchenManager
{
    private readonly IProducerMassTransit _producerMassTransit;
    public KitchenManager(IProducerMassTransit producerMassTransit)
    {
        _producerMassTransit = producerMassTransit;
    }

    /// <summary>
    /// Метод проверки для кухни, готова ли она выполнить заказ
    /// </summary>
    /// <param name="orderId">Номер заказа</param>
    /// <param name="clientId">Номер клиента</param>
    /// <param name="tableNumber">Номер столика</param>
    /// <param name="comments">Комментарии к заказу</param>
    /// <param name="success">Готов столик или нет</param>
    public async Task CheckToKitchenReadyAsync(
        Guid orderId, 
        Guid clientId,
        int tableNumber,
        string? comments, 
        bool success)
    {
        await _producerMassTransit.PublishToNotificationsAsync(orderId, clientId, tableNumber, comments, success);
    }

    /// <summary>
    /// Просто метод "проверки" готова ли кухня
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CheckIsKitchenReadyAsync()
    {
        return true;
    }
}