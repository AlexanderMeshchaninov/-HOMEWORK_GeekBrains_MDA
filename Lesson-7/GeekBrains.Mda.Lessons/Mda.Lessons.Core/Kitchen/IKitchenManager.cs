namespace Mda.Lessons.Core.Kitchen;

public interface IKitchenManager
{
    Task<bool> CheckIsKitchenReadyAsync();
}