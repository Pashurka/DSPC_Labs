using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static int vectorSize = 2128000000;
    static double[] vector = new double[vectorSize];

    static int numTests = 3;
    static int numberOfDelegates = 4;

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8;

        double[] results = new double[numTests];

        Console.WriteLine(
            "Лабораторна робота №6. Паралелізм завдань з використанням Parallel.Invoke.");

        Console.WriteLine($"Розмір вектору: {vectorSize}");
        Console.WriteLine($"Кількість делегатів: {numberOfDelegates}");

        // Генерація масиву виконується ДО тестових запусків
        Console.Write($"Генерація елементів масиву[1..{vectorSize}]...");

        GenerateVector();

        Console.WriteLine(" Масив сформовано...");
        Console.WriteLine("Тестові запуски розпочато...");

        // Тестові запуски
        for (int i = 0; i < numTests; i++)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Console.Write($"Тест {i + 1}: ");

            double result = CalculateVectorLength();

            stopwatch.Stop();

            Console.WriteLine(
                $".\nРезультат = {result:f0}, " +
                $"час = {stopwatch.ElapsedMilliseconds} мс.");

            results[i] = stopwatch.ElapsedMilliseconds;
        }

        double averageTime = results.Average();

        Console.WriteLine(
            $"Середній час виконання = {averageTime:f0} мс.");
    }

    // Паралельна генерація елементів масиву
    static void GenerateVector()
    {
        using ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random());

        int partSize = vectorSize / numberOfDelegates;

        Parallel.For(
            0,
            numberOfDelegates,
            part =>
            {
                int start = part * partSize;

                int end = (part == numberOfDelegates - 1)
                    ? vectorSize
                    : (part + 1) * partSize;

                Random r = random.Value!;

                for (int i = start; i < end; i++)
                {
                    vector[i] = r.NextDouble() * 10;
                }
            });
    }

    // Обчислення довжини вектору за допомогою Parallel.Invoke
    static double CalculateVectorLength()
    {
        double[] partialSums = new double[numberOfDelegates];

        Action[] actions = new Action[numberOfDelegates];

        int partSize = vectorSize / numberOfDelegates;

        for (int d = 0; d < numberOfDelegates; d++)
        {
            int delegateNumber = d;

            int start = d * partSize;

            int end = (d == numberOfDelegates - 1)
                ? vectorSize
                : (d + 1) * partSize;

            actions[d] = () =>
            {
                Console.Write(
                    $"[делегат {delegateNumber + 1}]");

                double sum = 0;

                for (int i = start; i < end; i++)
                {
                    double x = vector[i];

                    // Шість операцій для збільшення
                    // обчислювального навантаження
                    sum += Math.Sqrt(x * x);
                    sum += Math.Sqrt(x * x);
                    sum += Math.Sqrt(x * x);
                    sum += Math.Sqrt(x * x);
                    sum += Math.Sqrt(x * x);
                    sum += Math.Sqrt(x * x);
                }

                partialSums[delegateNumber] = sum;
            };
        }

        // Запуск усіх делегатів паралельно
        Parallel.Invoke(actions);

        // Об'єднання часткових результатів
        double sumOfSquares = partialSums.Sum();

        return sumOfSquares;
    }
}