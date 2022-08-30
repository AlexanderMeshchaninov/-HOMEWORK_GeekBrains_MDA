namespace Mda.Lessons.Core.RestaurantServices;

public interface INotificationService<TTable> where TTable : class
{
    Task NotifyBySmsAsync(TTable bookedTable, CancellationToken cancellationToken);
    Task NotifyCancelBySmsAsync(TTable canceledTable, CancellationToken cancellationToken);
    Task NotifyByPhoneAsync(TTable bookedTable, CancellationToken cancellationToken);
    Task NotifyCancelByPhoneAsync(TTable canceledTable, CancellationToken cancellationToken);
}