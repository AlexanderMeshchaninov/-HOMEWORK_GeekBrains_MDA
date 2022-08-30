using MassTransit;

namespace Mda.Lessons.Core.Consumers;

public interface IMakeIdempotentConsumer<TMessage> where TMessage : class
{
    Task MakeConsumerIdempotentInMemoryAsync(ConsumeContext<TMessage> context);
}