using LabWork7.BusinessLayer.Event_Args;

namespace LabWork7.BusinessLayer.PICalculators
{
    public class AsyncPIParallelCalculator : IAsyncPICalculator
    {
        private readonly List<Task<double>> m_tasks = new List<Task<double>>();
        private CancellationTokenSource m_cancellationTokenSource;

        public async Task<PiCalculationResult> CalculateAsync(
            long numOfSteps, int numOfThreads, IProgress<double> progress = null)
        {
            object percentIncrementLock = new object();
            double h = (double)1/(double)numOfSteps;
            double percentIncrement = 100 / numOfThreads;
            long chunk = numOfSteps/numOfThreads;
            double percentTotal = 0;
            m_cancellationTokenSource = new CancellationTokenSource();
            for (long i = 0; i < numOfThreads; ++i)
            { 
                long start = i * chunk;
                long end = (i == numOfThreads - 1) ? numOfSteps : (i + 1) * chunk;

                Task<double> t = new Task<double>(() => 
                {
                    var partSum = Sum(start, end, h, m_cancellationTokenSource);
                    if (progress != null)
                    {
                        lock (percentIncrementLock)
                        {
                            percentTotal += percentIncrement;
                            progress.Report(percentTotal);
                        }
                    }

                    return partSum;
                });

                m_tasks.Add(t);
            }

            //Start

            for (int i = 0; i < numOfThreads; ++i)
            {
                m_tasks[i].Start();
            }

            //Wait for execution
            await Task.WhenAll(m_tasks);
            double totalSum = 0;
            Exception exception = null;
            PiCalculationResult result;
            //Execution Finished
            try
            {
                for (int i = 0; i < m_tasks.Count; ++i)
                {
                    totalSum += m_tasks[i].Result;
                }
            }
            catch (AggregateException ex)
            {
                ex.Flatten().Handle(x =>
                {
                    if (x is OperationCanceledException opc)
                    {
                        exception = opc;
                        return true;
                    }

                    return false;
                });
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            { 
                result = new PiCalculationResult(totalSum * h, exception);
                m_tasks.Clear();
            }

            return result;
        }

        public async Task CancelCalculationAsync()
        {
            m_cancellationTokenSource.Cancel();
            await Task.Run(() =>
            {
                try
                {
                    Task.WaitAll(m_tasks);//Wait when all tasks will stop
                    m_tasks.Clear();
                }
                catch (AggregateException)
                {

                }
            });
        }

        protected virtual double Sum(long begin, long end, double step,
            CancellationTokenSource cancellationTokenSource)
        {
            double sum = 0;
            for (long i = begin; i < end; ++i)
            {
                double x = (i + 0.5) * step;
                sum += 4.0 / (1.0 + x * x);
                if(cancellationTokenSource.IsCancellationRequested)
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            return sum;
        }
    }
}
