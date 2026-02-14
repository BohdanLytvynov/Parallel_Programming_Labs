using LabWork3.Consumers;
using LabWork3.Results.Base;

namespace LabWork3.WorkQueues
{
    internal interface IWorkingQueue<TResultClass, TResult> : IDisposable
        where TResultClass : IConsumerResult<TResult>, new()
    {
        /// <summary>
        /// Indicates that pool is running
        /// </summary>
        bool IsStopped { get; }

        AutoResetEvent WaitHandle { get; }

        /// <summary>
        /// Add the work to the Queue for execution
        /// </summary>
        /// <param name="task">Task to execute</param>
        /// <param name="args">Arguments for task</param>
        /// <param name="onCompleted">callback that will send result</param>
        void Enqueue(Work<TResultClass, TResult> task, object args, Action<TResultClass> onCompleted);

        /// <summary>
        /// Must be called by the Consumers to get the task for execution
        /// </summary>
        /// <param name="work">task that must be done</param>
        /// <param name="args">Additional arguments for the task</param>
        /// <param name="callback">Result callback</param>
        /// <returns>true if task was taken</returns>
        bool TryGetTask(out Work<TResultClass, TResult> work, out object args, out Action<TResultClass> callback);
    }
}
