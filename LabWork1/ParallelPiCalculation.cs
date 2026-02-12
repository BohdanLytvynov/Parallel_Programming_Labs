using LabWork1.Interfaces;
using System.Diagnostics;

namespace LabWork1
{
    internal class ParallelPiCalculation : SyncPiCalculation, IPICalculator
    {
        #region Fields
        public List<Task<double>> Tasks { get; set; } = new List<Task<double>>();
        public int TaskCount { get; }
        #endregion

        #region Ctor
        public ParallelPiCalculation(int taskCount = 2)
        {
            TaskCount = taskCount;
        }
        #endregion

        public override double Calculate(long numOfSteps, out long time)
        {
            time = 0;

            double sum = 0;
            var step = (double)1 / (double)numOfSteps;
            //Init Tasks

            for (int i = 0; i < TaskCount; ++i)//O(T)
            {
                long start = (long)(numOfSteps * ((double)i/(double)TaskCount));
                long end  = (long)(numOfSteps * ((double)(i+1)/(double)TaskCount));
                Task<double> task = new Task<double>(() => Sum(start, end, step));
                Tasks.Add(task);
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < TaskCount; i++)
            {
                Tasks[i].Start();
            }

            Task.WaitAll(Tasks);

            for (int i = 0; i < TaskCount; ++i)//O(T)
            {
                sum += Tasks[i].Result;
            }

            stopwatch.Stop();

            time = stopwatch.ElapsedMilliseconds;

            return sum * step;
        }
    }

}
