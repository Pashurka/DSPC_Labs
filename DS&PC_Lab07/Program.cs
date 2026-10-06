using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    // Кількість доступних ядер CPU
    static int degreeOfParallelism = Environment.ProcessorCount;
    // ручне обмеження ступеню паралелізму
    // static int degreeOfParallelism = 4;

    // метод для паралельного обчислення довжини вектору
    static double CalculateSquaredLength(int[] vector)
    {
        // використанням методу AsParallel шаблону PLINQ
        return vector.AsParallel().WithDegreeOfParallelism(degreeOfParallelism)
                     .Select(x => x * x)
                     .Sum();

        /* упорядкування паралельного виконання запитів PLINQ
         return vector.AsParallel().AsOrdered()
             .WithDegreeOfParallelism(degreeOfParallelism)
             .Select(x => x * x)
             .Sum(); */
    }

    static void Main(string[] args)
    {
        int vectorDimension = 100000000; // розмірність вектору
        int numTests = 10; // кількість тестових запусків
        double totalExecutionTime = 0.0; // сумарний час виконання тестових запусків

        int[] vector = new int[vectorDimension];    // вектор цілих чисел

        Random random = new Random();
        for (int i = 0; i < vectorDimension; i++)
        {
            // обмеження значень елементів вектору
            vector[i] = random.Next(5);
        }

        Console.OutputEncoding = UTF8Encoding.UTF8; // підтримка укр. літер
        Console.WriteLine($"Ступень паралелізму = {degreeOfParallelism}");

        // організація тестових запусків
        for (int i = 0; i < numTests; i++)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            double squaredLength = Math.Sqrt(CalculateSquaredLength(vector));

            watch.Stop();

            double executionTime = watch.ElapsedMilliseconds;
            Console.WriteLine($"Спроба {i + 1}: довжина вектору {squaredLength:f0}," +
                $" час виконання = {executionTime} мс.");

            totalExecutionTime += executionTime;
        }

        // середній час виконання тестових запусків
        double averageExecutionTime = totalExecutionTime / numTests;
        Console.WriteLine($"Середній час виконання = {averageExecutionTime} мс.");

        Console.ReadLine();
    }
}
