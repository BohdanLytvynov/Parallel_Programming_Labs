using LabWork7.BusinessLayer.Event_Args;

namespace LabWork7.BusinessLayer.PICalculators
{
    public interface IAsyncPICalculator
    {
        EventHandler<OnPiCalculationFinishedArgs>? OnCalculationFinished { get; set; }
    }
}
