namespace Mda.Lessons.Dto;

public class RestaurantMenuStatus
{
    public string NotifyBySms { get; set; }
    public string NotifyByPhone { get; set; }
    public string CancelByPhone { get; set; }
    public string CancelBySms { get; set; }
    public string BookingQuestion { get; set; }
    public string CancelBookingQuestion { get; set; }
    public string RestaurantGratitude { get; set; }
}