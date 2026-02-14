using LabWork3.Results.Base;

namespace LabWork3.Consumers
{
    internal interface IConsumer<TResultClass, TResult> : IDisposable
        where TResultClass : IConsumerResult<TResult>, new()
    {

    }
}
