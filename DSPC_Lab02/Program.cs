using System.Diagnostics; // Для використання Stopwatch
using System.Text;
using System.Threading;

class Program
{
    static int n = 2128000000;               // кількість елементів вектора
    static double[] vector = new double[n]; // масив для зберігання значень вектора
    static double result = 0;               // обчислення квадрата довжини вектора
    static int numThreads = 12;              // кількість потоків
    static object lockObj = new object();   // об'єкт для блокування потоків

    // головна точка входу у програму
    static void Main()
    {
        // генератор випадкових величин
        Random random = new Random();
        double totalTime = 0; // Загальний час виконання

        Console.OutputEncoding = UTF8Encoding.UTF8; // підтримка укр. літер
        Console.WriteLine("Лабораторна робота №2. Багатопотокове обчислення з блокуванням lock().");
        Console.WriteLine($"Кількість потоків: {numThreads}");
        Console.Write($"Генерація елементів масиву[1..{ n}]...");

        // Заповнюємо вектор значеннями (випадковими числами)
        for (int i = 0; i < n; i++)
        {
            vector[i] = random.NextDouble() * 10;
        }

        Console.WriteLine($"Масив сформовано... \nТестові запуски розпочато...");

        // Виконуємо цикл для трьох тестових запусків
        for (int run = 1; run <= 3; run++)
        {
            result = 0; // Скидаємо результат перед кожним запуском

            // Створюємо та запускаємо Stopwatch для вимірювання часу
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Розподіл на потоки
            Thread[] threads = new Thread[numThreads];
            int chunkSize = n / numThreads;

            for (int t = 0; t < numThreads; t++)
            {
                int start = t * chunkSize;
                int end = (t == numThreads - 1) ? n : start + chunkSize;

                threads[t] = new Thread(() => CalculateSquareSum(start, end));
                threads[t].Start();
            }
            // Очікуємо завершення роботи всіх потоків
            foreach (var thread in threads)
            {
                thread.Join();
            }
            // Зупиняємо Stopwatch
            stopwatch.Stop();

            // Додаємо час виконання до загальної суми
            totalTime += stopwatch.ElapsedMilliseconds;

            // Виводимо результат обчислень для кожного запуску
            Console.WriteLine($"Запуск {run}:Сума квадратів елементів вектора = {result:F3}; " +
                $"час виконання = {(float)stopwatch.ElapsedMilliseconds / 1000} сек.");
        }

        // Обчислюємо середній час виконання
        double averageTime = totalTime / 3;
        // Виводимо середній час виконання
        Console.WriteLine($"Середній час виконання: {Math.Round(averageTime / 1000, 3)} сек.");
    }

    // Метод для обчислення суми квадратів елементів вектора у визначеному діапазоні
    static void CalculateSquareSum(int start, int end)
    {
        double localResult = 0;

        for (int i = start; i < end; i++)
        {
            double x = vector[i];
            localResult += Math.Sqrt(x * x);
            localResult += Math.Sqrt(x * x);
            localResult += Math.Sqrt(x * x);
            localResult += Math.Sqrt(x * x);
            localResult += Math.Sqrt(x * x);
            localResult += Math.Sqrt(x * x);
        }

        // Блокування при доступі до спільного ресурсу
        lock (lockObj)
        {
            result += localResult;
        }
    }
}
