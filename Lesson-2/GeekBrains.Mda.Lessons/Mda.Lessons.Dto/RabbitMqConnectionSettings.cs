namespace Mda.Lessons.Dto;

public sealed class RabbitMqConnectionSettings
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string HostName { get; set; }
    public int Port { get; set; }
}