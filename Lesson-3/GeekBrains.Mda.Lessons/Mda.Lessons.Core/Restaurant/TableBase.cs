namespace Mda.Lessons.Core.Restaurant;

public abstract class TableBase
{
    public int SeatsCount => _seatsCount;
    public State State => _state;
    public int TableNumber => _TableNumber;
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    private State _state { get; set; }
    private int _seatsCount { get; }
    private int _TableNumber { get; }
    
    public TableBase(int tableNumber)
    {
        Random rnd = new Random();
        
        _TableNumber = tableNumber;
        _state = State.Free;
        _seatsCount = rnd.Next(2, 6);
    }
    public bool SetState(State state)
    {
        if (State == state)
        {
            return false;
        }

        _state = state;
        return true;
    }
}