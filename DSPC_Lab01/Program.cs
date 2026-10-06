using System.Diagnostics;
using System.Text;

class Program
{
    static int n = 2128000000;               // кількість елементів вектора
    static double[] vector = new double[n]; // масив для зберігання значень вектора
    static double result = 0;               // сума квадратів елементів

    // головна точка входу у програму
    static void Main()
    {
        // генератор випадкових величин
        Random random = new Random();
        double totalTime = 0; // Загальний час виконання

        Console.OutputEncoding = UTF8Encoding.UTF8;
        Console.Write($"Лабораторна робота №1. Послідовний алгоритм.\nГенерація елементів масиву [1..{n}]...");

        // Заповнюємо вектор значеннями
        for (int i = 0; i < n; i++)
        {
            vector[i] = random.NextDouble() * 10;
        }

        Console.WriteLine("Масив сформовано.");
        Console.WriteLine("Тестові запуски розпочато...");

        // Виконуємо цикл для трьох тестових запусків
        for (int run = 1; run <= 3; run++)
        {
            result = 0;

            // Створюємо та запускаємо Stopwatch
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Послідовне обчислення суми квадратів
            for (int i = 0; i < n; i++)
            {
                double x = vector[i];

                result += Math.Sqrt(x * x);
                result += Math.Sqrt(x * x);
                result += Math.Sqrt(x * x);
                result += Math.Sqrt(x * x);
                result += Math.Sqrt(x * x);
                result += Math.Sqrt(x * x);
            }

            stopwatch.Stop();

            // Додаємо час виконання до загальної суми
            totalTime += stopwatch.ElapsedMilliseconds;

            // Виводимо результат
            Console.WriteLine(
                $"Запуск {run}: Сума квадратів елементів вектора = {result:F3}; " +
                $"час виконання = {(float)stopwatch.ElapsedMilliseconds / 1000} сек.");
        }

        // Обчислюємо середній час виконання
        double averageTime = totalTime / 3;

        Console.WriteLine(
            $"Середній час виконання: {Math.Round(averageTime / 1000, 3)} сек.");
    }
}
