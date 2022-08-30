namespace Mda.Lessons.Core.RestaurantServices;

public interface INotificationService<TRequest> where TRequest : class
{
    Task NotifyBySmsAsync(TRequest bookedTable);
    Task NotifyCancelBySmsAsync(TRequest canceledTable);
    Task NotifyByPhoneAsync(TRequest bookedTable);
    Task NotifyCancelByPhoneAsync(TRequest canceledTable);
}