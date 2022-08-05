// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using GeekBrains.Mda.Logic.Restaurant;

bool isWorking = true;

Restaurant restaurant = new Restaurant();

//---->Request by client
while (isWorking)
{
    Console.WriteLine();
    Console.WriteLine("Привет! Желаете забронировать столик?" +
                      "\n1 - Мы уведомим вас по СМС (асинхронно);" +
                      "\n2 - Подождите на линии, мы Вас оповестим по телефону (синхронно);" +
                      "\n3 - Чтобы снять бронь, по телефону (синхронно);" +
                      "\n4 - Чтобы снять бронь, по СМС (асинхронно)." +
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
            restaurant.BookingTableByPhone(InputValue("Сколько будет людей?"));
            break;
        case 3:
            restaurant.CancelBookingByPhone(InputValue("Назовите номер столика для снятия брони."));
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


