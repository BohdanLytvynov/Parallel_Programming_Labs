namespace LabWork3.Results.Base
{
    internal interface IConsumerResult<TResult>
    {
        bool Success { get; }
        Exception Exception { get; set; }
        TResult Result { get; set; }
        string Name { get; set; }
    }
}
