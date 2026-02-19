namespace LabWork5.MatrixCalculators
{
    public interface IMatrixCalculator
    {
        bool AddSubtractPossible(double[,] matA, double[,] matB);
        bool MultiplicationPossible(double[,] matA, double[,] matB);
        double[,] Add(double[,] matA, double[,] matB);
        double[,] Subtract(double[,] matA, double[,] matB);
        double[,] Multiply(double[,] matA, double[,] matB);

        void ParallelAdd(double[,] matA, double[,] matB,
            CancellationTokenSource cancellationTokenSource, IProgress<double>? progress = null);

        void ParallelSubtract(double[,] matA, double[,] matB,
            CancellationTokenSource cancellationTokenSource, IProgress<double>? progress = null);

        void ParallelMultiply(double[,] matA, double[,] matB,
            CancellationTokenSource cancellationTokenSource, IProgress<double>? progress = null);

        public event Action<string> OnParallelCalculationStarted;
        public event Action<string, double[,]> OnParallelCalculationFinished;

        public event Func<string, Task> OnParallelCalculationStartedAsync;
        public event Func<string, double[,], Task> OnParallelCalculationFinishedAsync;
    }

    public class MatrixCalculator : IMatrixCalculator
    {
        public event Func<string, Task> OnParallelCalculationStartedAsync;
        public event Func<string, double[,], Task> OnParallelCalculationFinishedAsync;

        public event Action<string> OnParallelCalculationStarted;
        public event Action<string, double[,]> OnParallelCalculationFinished;

        private int m_CPUCount;
        private bool m_parallelismUsed;
        private long m_rowsFinished;

        public bool Parallelism { get => m_parallelismUsed; }

        public MatrixCalculator()
        {
            m_parallelismUsed = false;
            m_CPUCount = Environment.ProcessorCount;
            m_rowsFinished = 0;
        }

        public bool AddSubtractPossible(double[,] matA, double[,] matB)
        {
            if(matA == null || matB == null) return false;
            return matA.GetLongLength(0) == matB.GetLongLength(0)
            && matA.GetLongLength(1) == matB.GetLongLength(1);
        }

        public bool MultiplicationPossible(double[,] matA, double[,] matB)
        { 
            if(matA == null || matB == null) return false;
            return matA.GetLongLength(1) == matB.GetLongLength(0);
        }

        public double[,] Add(double[,] matA, double[,] matB)
        {
            m_parallelismUsed = false;
            long rows = matA.GetLongLength(0);
            long cols = matA.GetLongLength(1);
            double[,] matC = new double[rows, cols];

            for (long j = 0; j < rows; ++j)
            {
                for (long k = 0; k < cols; ++k)
                {
                    matC[j, k] = matA[j, k] + matB[j, k];
                }
            }
            return matC;
        }

        public double[,] Subtract(double[,] matA, double[,] matB)
        {
            m_parallelismUsed = false;
            long rows = matA.GetLongLength(0);
            long cols = matA.GetLongLength(1);
            double[,] matC = new double[rows, cols];

            for (long j = 0; j < rows; ++j)
            {
                for (long k = 0; k < cols; ++k)
                {
                    matC[j, k] = matA[j, k] - matB[j, k];
                }
            }

            return matC;
        }

        public double[,] Multiply(double[,] matA, double[,] matB)
        {
            m_parallelismUsed = false;
            long rowsA = matA.GetLongLength(0);
            long colsA = matA.GetLongLength(1);
            long rowsB = matB.GetLongLength(0);
            long colsB = matB.GetLongLength(1);
            double[,] matC = new double[rowsA, colsB];

            for (int i = 0; i < rowsA; i++)
            {
                for (int j = 0; j < colsB; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < colsA; k++)
                    {
                        sum += matA[i, k] * matB[k, j];
                    }
                    matC[i, j] = sum;
                }
            }

            return matC;
        }

        public void ParallelAdd(double[,] matA, double[,] matB, 
            CancellationTokenSource cancellationTokenSource, IProgress<double>? progress = null)
        {
            long rows = matA.GetLongLength(0);
            long cols = matA.GetLongLength(1);
            double[,] matC = new double[rows, cols];
            long chunkSize = rows/m_CPUCount;
            m_rowsFinished = 0;
            m_parallelismUsed = true;
            List<Task> calculationTasks = new List<Task>(m_CPUCount);
            for (int i = 0; i < m_CPUCount; i++)
            {
                long start = i * chunkSize;
                long end = (i == m_CPUCount - 1)? rows : (i + 1) * chunkSize;

                var t = Task.Run(() =>
                    {
                        for (long j = start; j < end; ++j)
                        {
                            if (cancellationTokenSource.IsCancellationRequested)
                                break;

                            for (long k = 0; k < cols; ++k)
                            {
                                matC[j,k] = matA[j,k] + matB[j,k];
                            }

                            Report(progress, rows);
                        }
                    });

                calculationTasks.Add(t);
            }

            Task.WhenAll(calculationTasks).ContinueWith(x =>
            {
                OnParallelCalculationFinished?.Invoke("Addition", matC);
                OnParallelCalculationFinishedAsync?.Invoke("Addition", matC);
                m_parallelismUsed = false;
                calculationTasks.Clear();
            });

            OnParallelCalculationStarted?.Invoke("Addition");
            OnParallelCalculationStartedAsync?.Invoke("Addition");
        }

        public void ParallelSubtract(double[,] matA, double[,] matB,
            CancellationTokenSource cancellationTokenSource, IProgress<double>? progress = null)
        {
            long rows = matA.GetLongLength(0);
            long cols = matA.GetLongLength(1);
            double[,] matC = new double[rows, cols];
            long chunkSize = rows / m_CPUCount;
            m_rowsFinished = 0;
            m_parallelismUsed = true;
            var calculationTasks = new List<Task>(m_CPUCount);
            for (int i = 0; i < m_CPUCount; i++)
            {
                long start = i * chunkSize;
                long end = (i == m_CPUCount - 1) ? rows : (i + 1) * chunkSize;

                var t = Task.Run(() =>
                {
                    for (long j = start; j < end; ++j)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                            break;

                        for (long k = 0; k < cols; ++k)
                        {
                            matC[j, k] = matA[j, k] - matB[j, k];
                        }

                        Report(progress, rows);
                    }
                });

                calculationTasks.Add(t);
            }

            Task.WhenAll(calculationTasks)
                .ContinueWith(x =>
                {
                    OnParallelCalculationFinished?.Invoke("Subtraction", matC);
                    OnParallelCalculationFinishedAsync?.Invoke("Subtraction", matC);
                    m_parallelismUsed = false;
                    calculationTasks.Clear();
                });
            OnParallelCalculationStarted?.Invoke("Subtraction");
            OnParallelCalculationStartedAsync?.Invoke("Subtraction");
        }

        public void ParallelMultiply(double[,] matA, double[,] matB,
            CancellationTokenSource cancellationTokenSource, IProgress<double>? progress = null)
        {
            long rowsA = matA.GetLongLength(0);
            long colsA = matA.GetLongLength(1);
            long rowsB = matB.GetLongLength(0);
            long colsB = matB.GetLongLength(1);
            double[,] matC = new double[rowsA, colsB];
            long chunkSize = rowsA / m_CPUCount;
            m_rowsFinished = 0;
            m_parallelismUsed = true;
            var calculationTasks = new List<Task>(m_CPUCount);
            for (int i = 0; i < m_CPUCount; i++)
            {
                long start = i * chunkSize;
                long end = (i == m_CPUCount - 1) ? rowsA : (i + 1) * chunkSize;

                var t = Task.Run(() =>
                {
                    for (long iRow = start; iRow < end; iRow++)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                            break;

                        for (int kCol = 0; kCol < rowsB; kCol++)
                        {
                            double tempA = matA[iRow, kCol];
                            for (int jCol = 0; jCol < colsB; jCol++)
                            {
                                matC[iRow, jCol] += tempA * matB[kCol, jCol];
                            }
                        }

                        Report(progress, rowsA);
                    }
                });

                calculationTasks.Add(t);
            }

            Task.WhenAll(calculationTasks)
                .ContinueWith(x =>
                {
                    OnParallelCalculationFinished?.Invoke("Multiplication", matC);
                    OnParallelCalculationFinishedAsync?.Invoke("Multiplication", matC);
                    m_parallelismUsed = false;
                    calculationTasks.Clear();
                });
            OnParallelCalculationStarted?.Invoke("Multiplication");
            OnParallelCalculationStartedAsync?.Invoke("Multiplication");
        }

        private void Report(IProgress<double> progress, long rowsTotal)
        {
            if (progress == null) return;

            Interlocked.Increment(ref m_rowsFinished);

            if (m_rowsFinished % 200 == 0)
            {
                progress.Report((double)m_rowsFinished / rowsTotal * 100);
            }
        }
    }
}
