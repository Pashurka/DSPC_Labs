using System;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

// Dispatcher для розподілених обчислень
class Program
{
    // Загальний розмір вектора, який оброблятимуть Worker
    public static int vectorSize = 2_128_000_000;

    // Кількість клієнтів, які будуть задіяні в обробці
    static int workersToTest = 4;

    static async Task Main()
    {
        // Налаштування кодування консолі
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Лабораторна робота №9. Розподілені обчислення засобами ASP.Net Core.");
        Console.WriteLine("Запуск сервера.");
        Console.WriteLine($"Розмір вектора: {vectorSize:N0} елементів.");

        // Адреси чотирьох Worker
        string[] workers =
        {
            "http://localhost:5001/compute",
            "http://localhost:5002/compute",
            "http://localhost:5003/compute",
            "http://localhost:5004/compute"
        };

        // HTTP-клієнт для надсилання запитів Worker
        using var client = new HttpClient();

        // Перевірка продуктивності для 1, 2 та 4 Worker
        for (int k = 1; k <= workersToTest; k *= 2)
        {
            Console.WriteLine($"\nКількість клієнтів: {k}");

            // Розмір діапазону для одного Worker
            int chunkSize = vectorSize / k;

            // Список завдань для паралельного виконання HTTP-запитів
            var tasks = new List<Task<double>>();

            // Запуск вимірювання часу
            var sw = Stopwatch.StartNew();

            // Формування та надсилання запитів Worker
            for (int i = 0; i < k; i++)
            {
                // Початковий індекс діапазону
                int start = i * chunkSize;

                // Кінцевий індекс; останній Worker обробляє залишок
                int end = (i == k - 1)
                    ? vectorSize
                    : start + chunkSize;

                Console.WriteLine(
                    $"Кліент №{i + 1}: діапазон вектора [{start:N0}..{end:N0})");

                // Формування запиту з розміром вектора та межами діапазону
                var req = new ComputeRequest(vectorSize, start, end);

                // Надсилання запиту без очікування завершення попереднього
                tasks.Add(ComputeAsync(client, workers[i], req, i + 1));
            }

            // Очікування завершення всіх Worker
            double[] results = await Task.WhenAll(tasks);

            // Обчислення загального результату
            double totalResult = 0;

            foreach (double result in results)
                totalResult += result;

            // Завершення вимірювання часу
            sw.Stop();

            Console.WriteLine($"Загальний результат обчислення: {totalResult:E}");
            Console.WriteLine($"Час виконання: {sw.Elapsed.TotalSeconds:F3} с");
        }

        Console.WriteLine("Сервер завершив роботу, можна закрити вікно.");
    }

    // Надсилання запиту Worker та отримання часткового результату
    static async Task<double> ComputeAsync(
        HttpClient client,
        string url,
        ComputeRequest req,
        int workerNumber)
    {
        // Надсилання HTTP POST-запиту
        using var response = await client.PostAsJsonAsync(url, req);

        // Перевірка успішності HTTP-відповіді
        response.EnsureSuccessStatusCode();

        // Отримання часткової суми у форматі JSON
        double result = await response.Content.ReadFromJsonAsync<double>();

        Console.WriteLine(
            $"Worker {workerNumber} завершив обчислення: {result:E}");

        return result;
    }
}

// Модель запиту, яку Dispatcher передає Worker
record ComputeRequest(int VectorSize, int Start, int End);
