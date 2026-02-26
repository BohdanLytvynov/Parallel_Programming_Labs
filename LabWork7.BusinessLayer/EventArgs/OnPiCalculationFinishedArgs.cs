namespace LabWork7.BusinessLayer.Event_Args
{
    public class OnPiCalculationFinishedArgs : EventArgs
    {
        public double Value { get; set; } = double.MaxValue;

        public Exception? Exception { get; set; }

        public bool Success { get => Exception is null; }
    }
}
