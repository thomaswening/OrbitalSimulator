using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbitalSimulator.Utilities
{
    internal class Clock
    {
        #region Fields

        static readonly int dayInSeconds = 24 * 60 * 60;
        static readonly int hourInSeconds = 60 * 60;
        static readonly int minuteInSeconds = 60;
        static readonly int yearInSeconds = 365 * 24 * 60 * 60;

        readonly double elapsedSeconds = 0;
        public double ElapsedSeconds => elapsedSeconds;

        #endregion Fields

        public Clock(Func<Task> pMethodAsync)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            pMethodAsync().Wait();
            elapsedSeconds = stopwatch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine($"\nElapsed runtime >>> {ToString(elapsedSeconds)} ({elapsedSeconds} s)");
        }

        public Clock(Action pMethod)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            pMethod();
            elapsedSeconds = stopwatch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine($"\nElapsed runtime >>> {ToString(elapsedSeconds)} ({elapsedSeconds} s)");
        }

        #region Methods

        public static string ToString(double elapsedSeconds)
        {
            int years;
            int days;
            int hours;
            int minutes;
            double remainderInSeconds;

            years = (int)Math.Floor(elapsedSeconds / yearInSeconds);
            remainderInSeconds = elapsedSeconds % yearInSeconds;

            days = (int)Math.Floor(remainderInSeconds / dayInSeconds);
            remainderInSeconds %= dayInSeconds;

            hours = (int)Math.Floor(remainderInSeconds / hourInSeconds);
            remainderInSeconds %= hourInSeconds;

            minutes = (int)Math.Floor(remainderInSeconds / minuteInSeconds);
            remainderInSeconds %= minuteInSeconds;

            StringBuilder sb = new();

            if (years > 0) sb.Append($"{years} yr ");
            if (days > 0) sb.Append($"{days} d ");
            if (hours > 0) sb.Append($"{hours} h ");
            if (minutes > 0) sb.Append($"{minutes} min ");
            if (remainderInSeconds > 0) sb.Append(remainderInSeconds.ToString("0.00") + " s");

            return sb.Length != 0 ? sb.ToString() : "0.00 s";
        }

        public static async Task WaitAsync(int seconds, double fastForwardCoeff = 1.0)
        {
            int milliseconds = (int)Math.Round(1000 * seconds / fastForwardCoeff);
            await Task.Delay(milliseconds);
        }

        public static void Wait(int seconds, double fastForwardCoeff = 1.0)
        {
            int milliseconds = (int)Math.Round(1000 * seconds / fastForwardCoeff);
            Thread.Sleep(milliseconds);
        }

        #endregion Methods
    }
}
