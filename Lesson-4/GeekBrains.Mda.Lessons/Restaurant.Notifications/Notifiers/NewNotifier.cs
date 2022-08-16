namespace Restaurant.Notifications.Notifiers;

public class NewNotifier
{
    public void Notify(Guid orderId, Guid clientId, string message)
    {
        Console.WriteLine($"[X] [OrderID: {orderId}] Уважаемый клиент, {clientId}! {message}");
    }
}