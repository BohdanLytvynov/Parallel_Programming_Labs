namespace LabWork3.Results.Base
{
    internal class ConsumerResult<TResult> : IConsumerResult<TResult>
    {
        public Exception Exception { get; set; }

        public TResult Result { get; set; }

        public bool Success { get => Exception == null; }
        public string Name { get; set; }

        public ConsumerResult(TResult result, Exception exception)
        {
            Exception = exception;
            Result = result;
        }

        public ConsumerResult(TResult result) : this(result, null)
        {
            
        }

        public ConsumerResult() : this(default, null)
        {
            
        }
    }
}
