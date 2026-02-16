namespace LabWork5.RandomMatrixFillers
{
    internal class RandomMatrixFiller
    {
        private Random m_random;
        private int m_min;
        private int m_max;

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
    }
}
