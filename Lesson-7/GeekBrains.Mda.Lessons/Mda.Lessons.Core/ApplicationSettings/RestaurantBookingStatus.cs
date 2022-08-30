namespace Mda.Lessons.Core.ApplicationSettings;

public class RestaurantBookingStatus
{
    public string? Booked { get; set; }
    public string? Canceled { get; set; }
    public string? AutoCanceled { get; set; }
    public string? AlreadyExists { get; set; }
    public string? BookTableSmsGreeting { get; set; }
    public string? BookTablePhoneGreeting { get; set; }
    public string? CancelTableGreeting { get; set; }
}