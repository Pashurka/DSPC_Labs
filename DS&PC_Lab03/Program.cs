
using System.Diagnostics;
using System.Text;
using System.Threading;

class Program
{
    static int n = 2128000000; // Розмір вектора
    static int numThreads = 4; // Кількість потоків
    static double[] vector; // Вектор для обчислень
    static double result = 0; // Результат обчислень
    static Mutex mutex = new Mutex(); // М'ютекс для ексклюзивного доступу до результату

    static void Main(string[] args)
    {
        double totalTime = 0; // Загальний час виконання
        vector = new double[n];
        Random random = new Random();
        Console.OutputEncoding = UTF8Encoding.UTF8; // підтримка укр. літер
        Console.WriteLine("Лабораторна робота №3. Багатопотокове обчислення з м'ютексом.");
        Console.WriteLine($"Кількість потоків: {numThreads}");
        Console.Write($"Генерація елементів масиву[1..{n}]...");
        
        // Заповнюємо вектор випадковими значеннями
        for (int i = 0; i < n; i++)
        {
            vector[i] = random.NextDouble()*10;
        }

        Console.WriteLine($"Масив сформовано... \nТестові запуски розпочато...");
        Stopwatch stopwatch = new Stopwatch();

        for (int i = 0; i < 3; i++)
        {
            result = 0;
            stopwatch.Restart();

            Thread[] threads = new Thread[numThreads];

            // Запускаємо потоки для обчислення суми квадратів елементів вектора
            for (int j = 0; j < numThreads; j++)
            {
                threads[j] = new Thread(CalculateSquareLength);
                threads[j].Start(j);
            }

            // Очікуємо завершення всіх потоків
            foreach (var thread in threads)
            {
                thread.Join();
            }

            stopwatch.Stop();

            Console.WriteLine($"Тест {i + 1}: Квадрат довжини вектора: {result}, " +
                $"час виконання: {(float)stopwatch.ElapsedMilliseconds / 1000} сек");
            // Додаємо час виконання до загальної суми
            totalTime += stopwatch.ElapsedMilliseconds;
        }
        // Обчислюємо середній час виконання
        double averageTime = totalTime / 3;
        // Виводимо середній час виконання
        Console.WriteLine($"Середній час виконання: {Math.Round(averageTime / 1000, 3)} сек.");
    }

    static void CalculateSquareLength(object threadIndex)
    {
        int startIndex = (int)threadIndex * (n / numThreads);
        int endIndex = ((int)threadIndex + 1) * (n / numThreads);

        double threadResult = 0;

        // Обчислення суми квадратів частини вектора
        for (int i = startIndex; i < endIndex; i++)
        {
            double x = vector[i];
            threadResult += Math.Sqrt(x * x);
            threadResult += Math.Sqrt(x * x);
            threadResult += Math.Sqrt(x * x);
            threadResult += Math.Sqrt(x * x);
            threadResult += Math.Sqrt(x * x);
            threadResult += Math.Sqrt(x * x);
        }

        // Заблоковуємо м'ютекс, щоб оновити загальний результат
        mutex.WaitOne();
        result += threadResult;
        mutex.ReleaseMutex();
    }
}
