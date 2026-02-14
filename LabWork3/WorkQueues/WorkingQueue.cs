using LabWork3.Consumers;
using LabWork3.Results.Base;

namespace LabWork3.WorkQueues
{
    //AutoResetEvent
    //AutoResetEvent - is a controller of the threads. It has 2 main functions
    //WaitOne() - Thread will sleep and wait signal for activation
    //Set() - Release Thread that is waiting and it will be ready for work

    internal class WorkingQueue<TResultClass, TResult> : IWorkingQueue<TResultClass, TResult>
    where TResultClass : IConsumerResult<TResult>, new()
    {
        #region Fields

        private readonly Queue<TaskEntry> m_globalQueue;//Task queue
        private readonly List<Consumer<TResultClass, TResult>> m_workers;//List of thread wrappers
        private readonly AutoResetEvent m_waitHandle;//Control of thread awakening
        private readonly object m_locker; //Locker for teh queue, make Queue concurrent
        private bool m_isStopped; //all threads are stopped?

        #endregion

        #region Properties

        public AutoResetEvent WaitHandle => m_waitHandle;
        public bool IsStopped => m_isStopped;

        #endregion

        #region TaskEntry

        /// <summary>
        /// Struct that holds data about the Task
        /// </summary>
        private struct TaskEntry
        {
            /// <summary>
            /// Work to be done
            /// </summary>
            public Work<TResultClass, TResult> WorkDelegate;
            /// <summary>
            /// Args that must be passed to the task
            /// </summary>
            public object Args;
            /// <summary>
            /// Callback, will be executed after task will be finished
            /// </summary>
            public Action<TResultClass> Callback;
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="threadCount">Count of initial threads</param>
        public WorkingQueue(int threadCount)
        {
            m_globalQueue = new Queue<TaskEntry>();
            m_workers = new List<Consumer<TResultClass, TResult>>();
            m_waitHandle = new AutoResetEvent(false);
            m_locker = new object();
            m_isStopped = false;

            for (int i = 0; i < threadCount; i++)
            {
                // Init some amount of workers
                m_workers.Add(new Consumer<TResultClass, TResult>($"Worker-{i+1}", this));
            }
        }

        #endregion

        /// <summary>
        /// Add the work to the Queue for execution
        /// </summary>
        /// <param name="task">Task to execute</param>
        /// <param name="args">Arguments for task</param>
        /// <param name="onCompleted">callback that will send result</param>
        public void Enqueue(Work<TResultClass, TResult> task, object args, Action<TResultClass> onCompleted)
        {
            //lock the queue 
            lock (m_locker)
            {
                //Add new task data to the task queue
                m_globalQueue.Enqueue(new TaskEntry
                {
                    WorkDelegate = task,
                    Args = args,
                    Callback = onCompleted
                });
            }
            m_waitHandle.Set();//Wake the Thread for execution of the task
        }

        /// <summary>
        /// Must be called by the Consumers to get the task for execution
        /// </summary>
        /// <param name="work">task that must be done</param>
        /// <param name="args">Additional arguments for the task</param>
        /// <param name="callback">Result callback</param>
        /// <returns>true if task was taken</returns>
        public bool TryGetTask(out Work<TResultClass, TResult> work, out object args, out Action<TResultClass> callback)
        {
            //Pre-init, we need it, cause we use out key word
            work = null; 
            args = null; 
            callback = null;
            lock (m_locker)//lock the queue
            {
                if (m_globalQueue.Count > 0)//if queue is not empty
                {
                    var entry = m_globalQueue.Dequeue();//get the task
                    work = entry.WorkDelegate;
                    args = entry.Args;
                    callback = entry.Callback;
                    return true;
                }
                return false;
            }
        }

        public void Dispose()
        {
            if (m_isStopped) return;

            m_isStopped = true;

            while (true)
            {
                lock (m_locker)
                {
                    if (m_globalQueue.Count == 0) break;
                }
                Thread.Sleep(50);
            }

            //we need to awake all the threads call Set method
            for (int i = 0; i < m_workers.Count; i++)
            {
                m_waitHandle.Set();
            }

            //Call dispose on all threads
            foreach (var worker in m_workers)
            {
                worker.Dispose();
            }

            //Clean working queue resources
            m_waitHandle.Dispose();
        }
    }
}
