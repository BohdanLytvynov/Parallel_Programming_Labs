using LabWork1.Interfaces;
using System.Diagnostics;

namespace LabWork1
{
    internal class SyncPiCalculation : IPICalculator
    {
        public SyncPiCalculation()
        {

        }
        /// <summary>
        /// Calculate PI according to the amount of Steps
        /// </summary>
        /// <param name="numOfSteps">Steps</param>
        /// <param name="time">Time elapsed after calculation finished (ms)</param>
        /// <returns></returns>
        public virtual double Calculate(long numOfSteps, out long time)
        {
            time = 0;

            if(numOfSteps == 0)
                return 0;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            //1 Get Step
            var step = (double)1 / (double)numOfSteps;

            double sum = Sum(0, numOfSteps, step);
            sw.Stop();
            time = sw.ElapsedMilliseconds;
            return step*sum;
        }

        protected virtual double Sum(long begin, long end, double step)
        {
            double sum = 0;
            for (long i = begin; i < end; ++i)
            {
                double x = (i + 0.5) * step;
                sum += 4.0 / (1.0 + x * x);
            }
            return sum;
        }
    }
}
