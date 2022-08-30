
namespace Restaurant.Notifications.Notifiers;

public class Notifier
{
    private readonly ILogger<Notifier> _logger;

    public Notifier(ILogger<Notifier> logger)
    {
        _logger = logger;
    }
    
    public void Notify(Guid orderId, Guid clientId, string message)
    {
        _logger.Log(LogLevel.Information,$"[XX] OrderId: [{orderId}]" +
                                        $"\n[XX] ClientId: [{clientId}]" +
                                        $"\n[XX] Уважаемый АДМИНИСТРАТОР, {message}");
    }
}