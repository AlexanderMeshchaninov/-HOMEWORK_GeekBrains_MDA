namespace Mda.Lessons.Core.Kitchen;

public interface IKitchenManager
{
    Task CheckToKitchenReadyAsync(Guid orderId, Guid clientId, int tableNumber, string comment, bool success);
}