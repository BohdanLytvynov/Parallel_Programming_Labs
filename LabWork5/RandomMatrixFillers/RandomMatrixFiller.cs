using System.Diagnostics;

namespace LabWork5.RandomMatrixFillers
{
    public interface IRandomMatrixFiller
    {
        void Fill(double[,] mat);

        Task FillAsync(double[,] mat, CancellationTokenSource tokenSource, IProgress<double> progress = null);
    }

    public class RandomMatrixFiller : IRandomMatrixFiller
    {
        private Random m_random;
        private int m_min;
        private int m_max;
        private long m_rowsFinished;

        public RandomMatrixFiller()
        {
            m_min = -1000;
            m_max = 1000;
            m_random = new Random();
        }

        /// <summary>
        /// Fill matrix
        /// </summary>
        /// <param name="mat"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Fill(double[,] mat)
        { 
            if(mat == null)
                throw new ArgumentNullException(nameof(mat));

            long rows = mat.GetLongLength(0);
            long cols = mat.GetLongLength(1);

            for (long i = 0; i < rows; ++i)
            {
                for (long j = 0; j < cols; ++j)
                {
                    mat[i,j] = m_random.Next(m_min, m_max+1);
                }
            }
        }

        public async Task FillAsync(double[,] mat, CancellationTokenSource cancellationTokenSource,
            IProgress<double> progress = null)
        {
            await Task.Run(() =>
            {
                if (mat == null)
                    throw new ArgumentNullException(nameof(mat));
                m_rowsFinished = 0;
                long rows = mat.GetLongLength(0);
                long cols = mat.GetLongLength(1);

                for (long i = 0; i < rows; ++i)
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                        break;

                    for (long j = 0; j < cols; ++j)
                    {
                        mat[i, j] = m_random.Next(m_min, m_max + 1);
                    }
                    Report(progress, rows);
                }
            });
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
