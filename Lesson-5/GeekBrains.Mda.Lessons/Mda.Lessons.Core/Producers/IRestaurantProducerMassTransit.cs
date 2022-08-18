namespace Mda.Lessons.Core.Producers;

public interface IRestaurantProducerMassTransit<TTable> where TTable : class
{ 
    Task PublishToKitchenTableBookedAsync(TTable tableInfo, string message, CancellationToken cancellationToken);
    Task PublishToKitchenTableCanceledAsync(TTable tableInfo, string message, CancellationToken cancellationToken);
}