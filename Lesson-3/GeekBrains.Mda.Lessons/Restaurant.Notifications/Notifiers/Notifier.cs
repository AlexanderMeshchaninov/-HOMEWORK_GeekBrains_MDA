using System.Collections.Concurrent;

namespace Restaurant.Notifications.Notifiers;

public sealed class Notifier
{
    private readonly ConcurrentDictionary<Guid, Tuple<Guid?, Accepted>> _state = new();
    
    public void Accept(Guid orderId, Accepted accepted, int tableNumber, string comments, Guid? clientId = null)
    {
        _state.AddOrUpdate(orderId, new Tuple<Guid?, Accepted>(clientId, accepted), 
            (guid, oldValue) => new Tuple<Guid?, Accepted>(oldValue.Item1 ?? clientId, oldValue.Item2 | accepted));

        Notify(orderId, tableNumber, comments);
    }
    
    private void Notify(Guid orderId, int tableNumber, string comments)
    {
        var booking = _state[orderId];

        switch (booking.Item2)
        {
            case Accepted.All:
                Console.WriteLine($"[X] Успешно забронировано для клиента {booking.Item1}: " +
                                  $"\n[X] Столик номер: {tableNumber} " +
                                  $"\n[X] Статус: {comments} ");
                _state.Remove(orderId, out _);
                break;
            case Accepted.Rejected:
                Console.WriteLine($"[X] Клиент {booking.Item1}, " +
                                  $"\n[X] Столик номер {tableNumber} " +
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