using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;

// Створення та налаштування вебзастосунку 4-го клієнта
var builder = WebApplication.CreateBuilder(args);

// Прив'язування Worker до порту 5004
builder.WebHost.UseUrls("http://localhost:5004");

var app = builder.Build();

// Налаштування кодування консолі
Console.OutputEncoding = Encoding.UTF8;

// Повідомлення про запуск Worker
Console.WriteLine("Запуск 4-го клієнта.");
Console.WriteLine("Очікування запитів від сервера...");

// Обробка HTTP POST-запитів на виконання обчислень
app.MapPost("/compute", (ComputeRequest req) =>
{
    // Перевірка коректності отриманих параметрів
    if (req.VectorSize <= 0 ||
        req.Start < 0 ||
        req.End < req.Start ||
        req.End > req.VectorSize)
    {
        return Results.BadRequest("Некоректні параметри обчислення.");
    }

    // Виведення повідомлення про початок ініціалізації
    Console.WriteLine("Ініціалізація вектора ...");

    // Створення масиву заданого Dispatcher розміру
    double[] vector = new double[req.VectorSize];

    // Генератор псевдовипадкових чисел із фіксованим зерном
    var rand = new Random(42);

    // Заповнення вектора псевдовипадковими значеннями
    for (int i = 0; i < vector.Length; i++)
    {
        vector[i] = rand.NextDouble();
    }

    // Повідомлення про завершення ініціалізації
    Console.WriteLine("Вектор ініціалізовано.");

    // Виведення інформації про призначений діапазон
    Console.WriteLine(
        $"Обробка діапазону [{req.Start}..{req.End}) на порту 5004.");

    // Обчислення суми квадратів елементів заданого діапазону
    double partialSum = 0;

    for (int i = req.Start; i < req.End; i++)
    {
        partialSum += vector[i] * vector[i];
    }

    // Виведення часткового результату
    Console.WriteLine($"Часткова сума: {partialSum:E}");

    // Повернення результату Dispatcher у форматі JSON
    return Results.Ok(partialSum);
});

// Запуск вебсервера та очікування запитів
app.Run();

// Модель запиту від Dispatcher
// VectorSize — розмір вектора
// Start — початковий індекс діапазону (включно)
// End — кінцевий індекс діапазону (не включно)
record ComputeRequest(int VectorSize, int Start, int End);