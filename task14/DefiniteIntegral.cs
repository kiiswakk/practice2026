using System;
using System.Threading;
namespace task14;
public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsnumber)
    {
        if (function == null)
            throw new ArgumentNullException(nameof(function), "Функция не может быть null.");
        if (threadsnumber <= 0)
            throw new ArgumentException("Количество потоков должно быть больше нуля.", nameof(threadsnumber));
        if (step <= 0)
            throw new ArgumentException("Шаг интегрирования должен быть положительным.", nameof(step));

        double totalResult = 0.0;

        int totalSteps = (int)Math.Floor((b - a) / step);
        if (totalSteps <= 0)
            return 0.0;

        int baseSteps = totalSteps / threadsnumber;
        int remainder = totalSteps % threadsnumber;

        using Barrier barrier = new Barrier(threadsnumber + 1);

        for (int i = 0; i < threadsnumber; i++)
        {
            int threadIndex = i; 

            Thread thread = new Thread(() =>
            {
                int startStep = threadIndex * baseSteps + Math.Min(threadIndex, remainder);
                int endStep = startStep + baseSteps + (threadIndex < remainder ? 1 : 0);

                if (startStep >= endStep)
                {
                    barrier.SignalAndWait();
                    return;
                }

                double localA = a + startStep * step;
                double localB = a + endStep * step;

                double localSum = (function(localA) + function(localB)) / 2.0;

                for (int j = startStep + 1; j < endStep; j++)
                {
                    localSum += function(a + j * step);
                }

                double localIntegralResult = localSum * step;
    
                SafeAdd(ref totalResult, localIntegralResult);

                barrier.SignalAndWait();
            });

            thread.Start();
        }

        barrier.SignalAndWait();

        return totalResult;
    }

    private static void SafeAdd(ref double location, double value)
    {
        double initial;
        double newValue;
        do
        {
            initial = location;
            newValue = initial + value;
        }
        while (initial != Interlocked.CompareExchange(ref location, newValue, initial));
    }
}