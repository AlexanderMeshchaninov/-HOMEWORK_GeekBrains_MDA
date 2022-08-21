using System.Diagnostics;
using Microsoft.Extensions.Configuration;

Console.OutputEncoding = System.Text.Encoding.UTF8;
bool isWorking = true;

IConfiguration Configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var restaurant = new Restaurant.Booking.Restaurant.Restaurant(Configuration);

while (isWorking)
{
    await Task.Delay(10000);

    Console.WriteLine();
    Console.WriteLine("Привет! Желаете забронировать столик?" +
                      "\n1 - Мы уведомим вас по СМС;" +
                      "\n2 - Подождите на линии, мы Вас оповестим по телефону;" +
                      "\n3 - Чтобы снять бронь, по телефону;" +
                      "\n4 - Чтобы снять бронь, по СМС." +
                      "\n5 - выйти.");

    int choice = InputValue("");
    if (choice is not (1 or 2 or 3 or 4 or 5))
    {
        Console.WriteLine("Введите пожалуйста варианты:\n1 и 2 для заказа столика.\n3 и 4 для снятия брони.\n5 - выйти");
        continue;
    }

    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    switch (choice)
    {
        case 1:
            await restaurant.BookingTableBySmsAsync(InputValue("Сколько будет людей?"));
            break;
        case 2:
            await restaurant.BookingTableByPhoneAsync(InputValue("Сколько будет людей?"));
            break;
        case 3:
            await restaurant.CancelBookingByPhoneAsync(InputValue("Назовите номер столика для снятия брони."));
            break;
        case 4:
            await restaurant.CancelBookingBySmsAsync(InputValue("Назовите номер столика для снятия брони."));
            break;
        case 5:
            isWorking = false;
            break;
    }

    Console.WriteLine("Спасибо за ваше обращение!");
    stopwatch.Stop();

    TimeSpan timeSpan = stopwatch.Elapsed;

    Console.WriteLine($"{timeSpan.Seconds:00}:{timeSpan.Milliseconds:00}");
}

static int InputValue(string text)
{
    Console.WriteLine(text);
    int.TryParse(Console.ReadLine(), out int count);
    return count;
}
