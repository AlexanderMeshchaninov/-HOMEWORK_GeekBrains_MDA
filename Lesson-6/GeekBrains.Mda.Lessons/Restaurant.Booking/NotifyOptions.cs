namespace Restaurant.Booking;

[Flags]
public enum NotifyOptions
{
    NotifyBySms = 1,
    NotifyByPhone = 2,
    CancelBySms = 3,
    CancelByPhone = 4,
}