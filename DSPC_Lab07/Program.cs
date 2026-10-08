using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static int vectorSize = 2128000000;   // Розмір вектору
    static double[] vector = new double[vectorSize]; // Масив елементів вектору
    // Кількість доступних логічних процесорів
    static int degreeOfParallelism = Environment.ProcessorCount;
    // static int degreeOfParallelism = 4; // або ручне обмеження ступеню паралелізму
    static int numTests = 3; // кількість тестових запусків
    static double totalExecutionTime = 0.0; // сумарний час виконання запусків

    // метод для паралельного обчислення довжини вектору
    static double CalculateSquaredLength(double[] vector)
    {
        // використанням методу AsParallel шаблону PLINQ
        return vector.AsParallel().WithDegreeOfParallelism(degreeOfParallelism)
                     .Select(x => Math.Sqrt(x * x)+ Math.Sqrt(x * x)+ Math.Sqrt(x * x)+ 
                     Math.Sqrt(x * x)+ Math.Sqrt(x * x)+ Math.Sqrt(x * x))
                     .Sum();

        /* упорядкування паралельного виконання запитів PLINQ
         return vector.AsParallel().AsOrdered()
             .WithDegreeOfParallelism(degreeOfParallelism)
             .Select(x => x * x)
             .Sum(); */
    }

    // Паралельна генерація елементів масиву
    static void GenerateVector()
    {
        using ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random());

        int partSize = vectorSize / degreeOfParallelism;

        Parallel.For(
            0,
            degreeOfParallelism,
            part =>
            {
                int start = part * partSize;

                int end = (part == degreeOfParallelism - 1)
                    ? vectorSize
                    : (part + 1) * partSize;

                Random r = random.Value!;

                for (int i = start; i < end; i++)
                {
                    vector[i] = r.NextDouble() * 10;
                }
            });
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8; // підтримка укр. літер
        Console.WriteLine("Лабораторна робота №7. Паралельні обчислення з використанням шаблону PLINQ.");
        Console.WriteLine($"Розмір вектору: {vectorSize}");
        Console.WriteLine($"Ступінь паралелізму: {degreeOfParallelism}");
        // Генерація масиву виконується ДО тестових запусків
        Console.Write($"Генерація елементів масиву[1..{vectorSize}]...");
        GenerateVector();
        Console.WriteLine(" Масив сформовано...");
        Console.WriteLine("Тестові запуски розпочато...");

        // організація тестових запусків
        for (int i = 0; i < numTests; i++)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            double squaredLength = CalculateSquaredLength(vector);
            watch.Stop();
            double executionTime = watch.ElapsedMilliseconds;
            Console.WriteLine($"Спроба {i + 1}: довжина вектору {squaredLength:f0}," +
                $" час виконання = {executionTime} мс.");

            totalExecutionTime += executionTime;
        }

        // середній час виконання тестових запусків
        double averageExecutionTime = totalExecutionTime / numTests;
        Console.WriteLine($"Середній час виконання = {averageExecutionTime} мс.");
    }
}
