using System.Diagnostics;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static int vectorSize = 2128000000;   // Розмір вектору
    static double[] vector = new double[vectorSize]; // Масив елементів вектору
    // Визначаємо кількість доступних процесорів
    // static int processorCount = Environment.ProcessorCount;
    static int processorCount = 1; // ручна зміна кількості процесорів

    static void Main()
    {
        int numTests = 5; // Кількість тестових запусків
        double[] results = new double[numTests]; // час виконання тестових запусків

        Console.OutputEncoding = UTF8Encoding.UTF8; // підтримка укр. літер
        Console.WriteLine("Лабораторна робота №5. Паралелізм даних з використанням бібліотеки TPL.");
        Console.WriteLine("Кількість логічних процесорів на ПК: {0}.", Environment.ProcessorCount);
        Console.WriteLine("Кількість потоків: {0}.", processorCount);
        Console.Write($"Генерація елементів масиву[1..{vectorSize}]...");

        // Заповнюємо вектор випадковими значеннями
        Random rand = new Random();
        for (int i = 0; i < vectorSize; i++)
        {
            vector[i] = rand.NextDouble()*10;
        }

        Console.WriteLine($"Масив сформовано... \nТестові запуски розпочато...");

        for (int i = 0; i < numTests; i++)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Виконуємо обчислення з використанням Parallel.For та зупинкою циклу 
            // методом Break
            double result = CalculateVectorLength(processorCount);

            stopwatch.Stop();
            Console.WriteLine($"Спроба {i + 1}: Результат = {result}, " +
                              $"Час = {stopwatch.ElapsedMilliseconds} мс");

            results[i] = stopwatch.ElapsedMilliseconds;
        }

        // Виводимо середній час виконання
        double averageTime = results.Average();
        Console.WriteLine($"Середній час = {averageTime} мс");
    }

    // Функція обчислення суми квадратів елементів вектору
    static double CalculateVectorLength(int processorCount)
    {
        double sumOfSquares = 0;

        // Обчислюємо квадрат довжини вектору з використанням Parallel.For
        Parallel.For(0, vectorSize, new ParallelOptions
        { MaxDegreeOfParallelism = processorCount }, (i, state) =>
        {
            double x = vector[i];
            sumOfSquares += Math.Sqrt(x * x);
            sumOfSquares += Math.Sqrt(x * x);
            sumOfSquares += Math.Sqrt(x * x);
            sumOfSquares += Math.Sqrt(x * x);
            sumOfSquares += Math.Sqrt(x * x);
            sumOfSquares += Math.Sqrt(x * x);
        });

        return sumOfSquares;
    }
}

