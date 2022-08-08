namespace Restaurant.Booking.Restaurant;

public sealed class Table
{
    public int SeatsCount => _seatsCount;
    public State State => _state;
    public int Id => _id;
    private State _state { get; set; }
    private int _seatsCount { get; }
    private int _id { get; }
    
    public Table(int id)
    {
        Random rnd = new Random();
        
        _id = id;
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