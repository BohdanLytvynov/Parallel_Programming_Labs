using LabWork7.BusinessLayer.Event_Args;

namespace LabWork7.BusinessLayer.PICalculators
{
    public interface IAsyncPICalculator
    {
        Task CancelCalculationAsync();
        Task<PiCalculationResult> CalculateAsync(
            long numOfSteps, int numOfThreads, 
            IProgress<double> progress = null);
    }
}
