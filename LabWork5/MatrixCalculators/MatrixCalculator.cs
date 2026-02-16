namespace LabWork5.MatrixCalculators
{
    internal class MatrixCalculator
    {
        public event Action<string> OnParallelCalculationStarted;
        public event Action<string, double[,]> OnParallelCalculationFinished;

        private int m_CPUCount;
        private bool m_parallelismUsed;

        public bool Parallelism { get => m_parallelismUsed; }

        public MatrixCalculator()
        {
            m_parallelismUsed = false;
            m_CPUCount = Environment.ProcessorCount;
        }

        public bool AddSubtractPossible(double[,] matA, double[,] matB)
            => matA.GetLongLength(0) == matB.GetLongLength(0)
            && matA.GetLongLength(1) == matB.GetLongLength(1);

        public bool MultiplicationPossible(double[,] matA, double[,] matB)
            => matA.GetLongLength(1) == matB.GetLongLength(0);

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
                    matC[j, k] = matA[j, k] + matB[j, k];
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
            CancellationTokenSource cancellationTokenSource)
        {
            long rows = matA.GetLongLength(0);
            long cols = matA.GetLongLength(1);
            double[,] matC = new double[rows, cols];
            long chunkSize = rows/m_CPUCount;

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
                        }
                    });

                calculationTasks.Add(t);
            }

            Task.WhenAll(calculationTasks).ContinueWith(x =>
            {
                OnParallelCalculationFinished?.Invoke("Addition", matC);
                calculationTasks.Clear();
            });
            OnParallelCalculationStarted?.Invoke("Addition");
        }

        public void ParallelSubtract(double[,] matA, double[,] matB,
            CancellationTokenSource cancellationTokenSource)
        {
            long rows = matA.GetLongLength(0);
            long cols = matA.GetLongLength(1);
            double[,] matC = new double[rows, cols];
            long chunkSize = rows / m_CPUCount;

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
                    }
                });

                calculationTasks.Add(t);
            }

            Task.WhenAll(calculationTasks)
                .ContinueWith(x =>
                {
                    OnParallelCalculationFinished?.Invoke("Subtraction", matC);
                    calculationTasks.Clear();
                });
            OnParallelCalculationStarted?.Invoke("Subtraction");
        }

        public void ParallelMultiply(double[,] matA, double[,] matB,
            CancellationTokenSource cancellationTokenSource)
        {
            long rowsA = matA.GetLongLength(0);
            long colsA = matA.GetLongLength(1);
            long rowsB = matB.GetLongLength(0);
            long colsB = matB.GetLongLength(1);
            double[,] matC = new double[rowsA, colsB];
            long chunkSize = rowsA / m_CPUCount;

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
                    }
                });

                calculationTasks.Add(t);
            }

            Task.WhenAll(calculationTasks)
                .ContinueWith(x =>
                {
                    OnParallelCalculationFinished?.Invoke("Multiplication", matC);
                    calculationTasks.Clear();
                });
            OnParallelCalculationStarted?.Invoke("Multiplication");
        }
    }
}
