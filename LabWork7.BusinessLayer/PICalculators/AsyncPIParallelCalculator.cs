using LabWork7.BusinessLayer.Event_Args;

namespace LabWork7.BusinessLayer.PICalculators
{
    public class AsyncPIParallelCalculator : IAsyncPICalculator
    {
        public EventHandler<OnPiCalculationFinishedArgs>? OnCalculationFinished { get; set; }



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
