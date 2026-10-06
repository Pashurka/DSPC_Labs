using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static int vectorSize = 10000000;   		// Розмір вектору
    static int[] vector = new int[vectorSize]; 	// Масив для зберігання координат вектору
    static int numTests = 9; 				// Кількість тестових запусків

    static void Main(string[] args)
    {
        Console.OutputEncoding = UTF8Encoding.UTF8; // підтримка укр. літер
        double[] results = new double[numTests];  	// масив вимірів часу виконання

        // Заповнюємо вектор випадковими значеннями
        Random rand = new Random();
        for (int i = 0; i < vectorSize; i++)
        {
            vector[i] = rand.Next(100);
        }

        for (int i = 0; i < numTests; i++)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            Console.Write($"Тест {i + 1}: ");
            // Виконуємо обчислення з використанням Parallel.Invoke
            double result = CalculateVectorLength();

            stopwatch.Stop();
            Console.WriteLine($", результат = {result:f0}, " +
                $"час = {stopwatch.ElapsedMilliseconds} мс.");

            results[i] = stopwatch.ElapsedMilliseconds;
        }

        // Виводимо середній час виконання
        double averageTime = results.Average();
        Console.WriteLine($"Середній час виконання = {averageTime:f0} мс.");
    }

    static double CalculateVectorLength()
    {
        double sumOfSquares = 0;    // сума квадратів елементів вектору

        // Розбиваємо обчислення на чотири делегата та викликаємо їх паралельно з 
        // використанням Parallel.Invoke
        Parallel.Invoke(
            () =>
            {
                // перший делегат Action
                Console.Write("[делегат 1]");

                for (int i = 0; i < vectorSize / 4 - 1; i++)
                {
                    double partialResult = vector[i] * vector[i];
                    // Забезпечуємо взаємний виключний доступ до змінної sumOfSquares
                    lock (vector)
                    {
                        sumOfSquares += partialResult;
                    }
                }
            },
            () =>
            {
                // другий делегат Action
                Console.Write("[делегат 2]");
                for (int i = vectorSize / 4; i < vectorSize / 2 - 1; i++)
                {
                    double partialResult = vector[i] * vector[i];
                    // Забезпечуємо взаємний виключний доступ до змінної sumOfSquares
                    lock (vector)
                    {
                        sumOfSquares += partialResult;
                    }
                }
            },
            () =>
            {
                // третій делегат Action
                Console.Write("[делегат 3]");

                for (int i = vectorSize / 2; i < vectorSize / 4 * 3 - 1; i++)
                {
                    double partialResult = vector[i] * vector[i];
                    // Забезпечуємо взаємний виключний доступ до змінної sumOfSquares
                    lock (vector)
                    {
                        sumOfSquares += partialResult;
                    }
                }
            },
            () =>
            {
                // четвертий делегат Action
                Console.Write("[делегат 4]");
                for (int i = vectorSize / 4 * 3; i < vectorSize; i++)
                {
                    double partialResult = vector[i] * vector[i];
                    // Забезпечуємо взаємний виключний доступ до змінної sumOfSquares
                    lock (vector)
                    {
                        sumOfSquares += partialResult;
                    }
                }
            }
        );

        return Math.Sqrt(sumOfSquares);
    }
}

