
namespace Restaurant.Messaging.Models;

public class BookingRequestModel
{
    public Guid OrderId { get; private set; }
    public Guid ClientId { get; private set; }

    private HashSet<string?> _messageIds = new HashSet<string?>();

    public BookingRequestModel(
        Guid orderId, 
        Guid clientId,
        string? messageId)
    {
        OrderId = orderId;
        ClientId = clientId;
        
        _messageIds.Add(messageId);
    }

    /// <summary>
    /// Метод обновления объекта
    /// </summary>
    /// <param name="model">Модель самого объекта</param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public BookingRequestModel Update(BookingRequestModel model, string? messageId)
    {
        _messageIds.Add(messageId);
        
        OrderId = model.OrderId;
        ClientId = model.ClientId;
        
        return this;
    }
    
    public bool CheckMessageId(string? messageId)
    {
        return _messageIds.Contains(messageId);
    }
}