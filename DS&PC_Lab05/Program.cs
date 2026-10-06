using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static int vectorSize = 700000000;   // Розмір вектору
    static double[] vector = new double[vectorSize]; // Масив елементів вектору
    // Визначаємо кількість доступних процесорів
    static int processorCount = Environment.ProcessorCount;
    // static int processorCount = 9; // ручна зміна кількості процесорів

    static void Main()
    {
        Console.OutputEncoding = UTF8Encoding.UTF8; // підтримка укр. літер
        Console.WriteLine("Кількість логічних процесорів на ПК: {0}.",
                           processorCount);

        int numTests = 5; // Кількість тестових запусків
        double[] results = new double[5]; // час виконання тестових запусків

        Console.WriteLine($"Кількість потоків: {processorCount}");
        Console.WriteLine("Генерація елементів масиву...");

        // Заповнюємо вектор випадковими значеннями
        Random rand = new Random();
        for (int i = 0; i < vectorSize; i++)
        {
            vector[i] = rand.NextDouble();
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

        // Обчислюємо квадрат довжини вектору з використанням Parallel.For та зупинкою 
        // циклу методом Break
        Parallel.For(0, vectorSize, new ParallelOptions
        { MaxDegreeOfParallelism = processorCount }, (i, state) =>
        {
            double partialResult = vector[i] * vector[i];
            // Забезпечуємо взаємний виключний доступ до змінної sumOfSquares
            lock (vector)
            {
                sumOfSquares += partialResult;
            }

            if (i == vectorSize - 1)
            {
                state.Break(); // Зупиняємо цикл після останньої ітерації
            }
        });

        return sumOfSquares;
    }
}

