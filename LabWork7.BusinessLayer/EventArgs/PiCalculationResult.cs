namespace LabWork7.BusinessLayer.Event_Args
{
    public struct PiCalculationResult
    {
        public double Value { get; set; }

        public Exception? Exception { get; set; }

        public bool Success { get => Exception is null; }

        public bool Cancelled 
        {
            get
            { 
                if(Exception == null)
                    return false;

                if(Exception is OperationCanceledException)
                    return true;

                return false;
            }
        }

        public PiCalculationResult() : this(double.MaxValue, null)
        {
        }

        public PiCalculationResult(double value, Exception ex)
        {
            Value = value;
            Exception = ex;
        }
    }
}
