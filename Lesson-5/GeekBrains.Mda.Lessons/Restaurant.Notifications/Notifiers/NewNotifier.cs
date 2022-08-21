namespace Restaurant.Notifications.Notifiers;

public class NewNotifier
{
    public void Notify(Guid orderId, Guid clientId, string message)
    {
        Console.WriteLine($"[XX] OrderId: [{orderId}]" +
                          $"\n[XX] ClientId: [{clientId}]" +
                          $"\n[XX] Уважаемый АДМИНИСТРАТОР, {message}");
    }
}