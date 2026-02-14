using LabWork3.Results.Base;
using LabWork3.WorkQueues;

namespace LabWork3.Consumers
{
    /// <summary>
    /// Delegate that contains the pointer to the task that must be done
    /// </summary>
    /// <typeparam name="TResultClass"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="args"></param>
    /// <param name="result"></param>
    internal delegate void Work<TResultClass, TResult>(object args, out TResultClass result)
        where TResultClass : IConsumerResult<TResult>, new();

    /// <summary>
    /// Thread Wrapper
    /// </summary>
    /// <typeparam name="TResultClass"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    internal class Consumer<TResultClass, TResult> : IConsumer<TResultClass, TResult>
    where TResultClass : IConsumerResult<TResult>, new()
    {
        private readonly Thread m_thread;//Thread that performs work
        private readonly IWorkingQueue<TResultClass, TResult> m_pool;//reference to thread pool

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pool"></param>
        public Consumer(string name, IWorkingQueue<TResultClass, TResult> pool)
        {
            m_pool = pool;
            m_thread = new Thread(WorkLoop) { Name = name, IsBackground = true };//Start thread with empty task
            m_thread.Start();
        }

        public void Dispose()
        {
            if (m_thread.IsAlive)
            {
                //Give thread 1s, so it can finish
                m_thread.Join(TimeSpan.FromSeconds(1));
            }
        }

        /// <summary>
        /// Empty work, that will run in a thread
        /// </summary>
        private void WorkLoop()
        {
            while (!m_pool.IsStopped)
            {
                //Try to get the work from the pool
                if (m_pool.TryGetTask(out var work, out var args, out var callback))
                {
                    TResultClass result = default;
                    try
                    {
                        work(args, out result);//Execute work
                    }
                    catch (Exception ex)//Catch an exception
                    {
                        if (result == null) result = new TResultClass();
                        result.Exception = ex;
                    }
                    finally
                    {
                        if (result == null) result = new TResultClass();
                        result.Name = m_thread.Name;
                    }

                    //Send result using callback model
                    callback?.Invoke(result);
                }
                else
                {
                    if (m_pool.IsStopped)
                    {
                        break;
                    }

                    try
                    {
                        //No task - thread must sleep
                        m_pool.WaitHandle.WaitOne();
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                }
            }
        }

    }
}
