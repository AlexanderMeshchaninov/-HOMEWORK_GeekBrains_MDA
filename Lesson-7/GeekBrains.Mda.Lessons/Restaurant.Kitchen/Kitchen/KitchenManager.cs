using Mda.Lessons.Core.Kitchen;

namespace Restaurant.Kitchen.Kitchen;

public class KitchenManager : IKitchenManager
{
    /// <summary>
    /// Просто метод "проверки" готова ли кухня
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CheckIsKitchenReadyAsync()
    {
        return true;
    }
}