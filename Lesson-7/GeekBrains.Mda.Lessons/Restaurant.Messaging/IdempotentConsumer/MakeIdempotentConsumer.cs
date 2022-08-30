using System.Data;
using MassTransit;
using Mda.Lessons.Core.Consumers;
using Mda.Lessons.Core.IMemoryRepository;
using Restaurant.Messaging.EventsContracts;
using Restaurant.Messaging.MemoryRepository;
using Restaurant.Messaging.Models;

namespace Restaurant.Messaging.IdempotentConsumer;

public interface IMakeIdempotentConsumer : 
    IMakeIdempotentConsumer<IBookingRequestEvent>, 
    IMakeIdempotentConsumer<IBookingInnerCancelEvent>,
    IMakeIdempotentConsumer<IBookingCancelRequestEvent>
{ }

public class MakeIdempotentConsumer : IMakeIdempotentConsumer
{
    private readonly IMemoryRepository<BookingRequestModel> _memoryRepository;

    public MakeIdempotentConsumer()
    {
        _memoryRepository = new MemoryRepository<BookingRequestModel>();
    }
    
    /// <summary>
    /// Метод (фасад) для кода проверки messageIds используется для создания некой идемподентности consumer
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <returns></returns>
    /// <exception cref="DuplicateNameException">Исключение если объект второй раз попал сюда</exception>
    public async Task MakeConsumerIdempotentInMemoryAsync(ConsumeContext<IBookingRequestEvent> context)
    {
        var model = _memoryRepository.Get().FirstOrDefault(x => x.OrderId.Equals(context.Message.OrderId));

        if (model is not null && model.CheckMessageId(context.MessageId.ToString()))
        {
            Console.WriteLine(context.MessageId.ToString());
            Console.WriteLine("Second time");

            throw new DuplicateNameException("Входящий объект уже был...");
        }
        
        var requestModel = new BookingRequestModel(
            context.Message.OrderId,
            context.Message.ClientId,
            context.MessageId.ToString());
            
        Console.WriteLine(context.MessageId.ToString());
        Console.WriteLine("First time");

        var resultModel = model?.Update(requestModel, context.MessageId.ToString()) ?? requestModel;
        await _memoryRepository.AddOrUpdateAsync(context.Message.OrderId, resultModel);
    }

    /// <summary>
    /// Метод (фасад) для кода проверки messageIds используется для создания некой идемподентности consumer
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <returns></returns>
    /// <exception cref="DuplicateNameException">Исключение если объект второй раз попал сюда</exception>
    public async Task MakeConsumerIdempotentInMemoryAsync(ConsumeContext<IBookingInnerCancelEvent> context)
    {
        var model = _memoryRepository.Get().FirstOrDefault(x => x.OrderId.Equals(context.Message.OrderId));

        if (model is not null && model.CheckMessageId(context.MessageId.ToString()))
        {
            Console.WriteLine(context.MessageId.ToString());
            Console.WriteLine("Second time");

            throw new DuplicateNameException("Входящий объект уже был...");
        }
        
        var requestModel = new BookingRequestModel(
            context.Message.OrderId, 
            context.Message.ClientId,
            context.MessageId.ToString());
            
        Console.WriteLine(context.MessageId.ToString());
        Console.WriteLine("First time");

        var resultModel = model?.Update(requestModel, context.MessageId.ToString()) ?? requestModel;
        await _memoryRepository.AddOrUpdateAsync(context.Message.OrderId, resultModel);
    }

    /// <summary>
    /// Метод (фасад) для кода проверки messageIds используется для создания некой идемподентности consumer
    /// </summary>
    /// <param name="context">Контекст</param>
    /// <returns></returns>
    /// <exception cref="DuplicateNameException">Исключение если объект второй раз попал сюда</exception>
    public async Task MakeConsumerIdempotentInMemoryAsync(ConsumeContext<IBookingCancelRequestEvent> context)
    {
        var model = _memoryRepository.Get().FirstOrDefault(x => x.OrderId.Equals(context.Message.OrderId));

        if (model is not null && model.CheckMessageId(context.MessageId.ToString()))
        {
            Console.WriteLine(context.MessageId.ToString());
            Console.WriteLine("Second time");

            throw new DuplicateNameException("Входящий объект уже был...");
        }
        
        var requestModel = new BookingRequestModel(
            context.Message.OrderId, 
            context.Message.ClientId,
            context.MessageId.ToString());
            
        Console.WriteLine(context.MessageId.ToString());
        Console.WriteLine("First time");

        var resultModel = model?.Update(requestModel, context.MessageId.ToString()) ?? requestModel;
        await _memoryRepository.AddOrUpdateAsync(context.Message.OrderId, resultModel);
    }
}