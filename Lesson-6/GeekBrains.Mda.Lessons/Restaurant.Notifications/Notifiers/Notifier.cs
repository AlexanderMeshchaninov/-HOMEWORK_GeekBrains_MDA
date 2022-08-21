using System.Collections.Concurrent;
using Mda.Lessons.Core.Restaurant;
using Restaurant.Messaging;

namespace Restaurant.Notifications.Notifiers;

public class Notifier
{
    private readonly ConcurrentDictionary<Guid, Tuple<Guid?, Accepted>> _state = new();
    
    public void Accept(Guid orderId, Accepted accepted, Dish preOrder, int tableNumber, string? comments, Guid? clientId = null)
    {
        _state.AddOrUpdate(orderId, new Tuple<Guid?, Accepted>(clientId, accepted), 
            (guid, oldValue) => new Tuple<Guid?, Accepted>(oldValue.Item1 ?? clientId, oldValue.Item2 | accepted));

        Notify(orderId, preOrder, tableNumber, comments);
    }
    
    private void Notify(Guid orderId, Dish preOrder, int tableNumber, string? comments)
    {
        var booking = _state[orderId];

        switch (booking.Item2)
        {
            case Accepted.All:
                Console.WriteLine($"[X] Успешно забронировано для клиента {booking.Item1}: " +
                                  $"\n[X] Столик номер: {tableNumber} " +
                                  $"\n[X] Предзаказ: {preOrder} " +
                                  $"\n[X] Статус: {comments} ");
                _state.Remove(orderId, out _);
                break;
            case Accepted.Rejected:
                Console.WriteLine($"[X] Клиент {booking.Item1}, " +
                                  $"\n[X] Столик номер: {tableNumber} " +
                                  $"\n[X] Предзаказ: {preOrder} " +
                                  $"\n[X] Статус: {comments} ");
                _state.Remove(orderId, out _);
                break;
            case Accepted.Kitchen:
            case Accepted.Booking:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}