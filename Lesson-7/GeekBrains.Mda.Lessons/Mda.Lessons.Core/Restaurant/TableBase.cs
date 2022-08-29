namespace Mda.Lessons.Core.Restaurant;

public abstract class TableBase
{
    public int SeatsCount => _seatsCount;
    public State State => _state;
    public int TableNumber => _tableNumber;
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    private State _state { get; set; }
    private int _seatsCount { get; }
    private int _tableNumber { get; }
    private object _locker = new object();
    
    public TableBase(int tableNumber)
    {
        Random rnd = new Random();
        
        _tableNumber = tableNumber;
        _state = State.Free;
        _seatsCount = rnd.Next(2, 6);
    }
    public bool SetState(State state)
    {
        lock (_locker)
        {
            if (State == state)
            {
                return false;
            }

            _state = state;
            return true;
        }
    }
}