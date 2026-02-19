using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LabWork5_AvaloniaUI.Helpers
{
    public static class UIHelper
    {
        /// <summary>
        /// Converts MatrixTo String
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        [Obsolete("Weak performance in UI!")]
        public static string MatToString(double[,] mat)
        {
            long rows = mat.GetLongLength(0);
            long cols = mat.GetLongLength(1);

            StringBuilder sb = new StringBuilder();

            for (long i = 0; i < rows; ++i)
            {
                for (long j = 0; j < cols; ++j)
                {
                    sb.Append(mat[i, j].ToString());
                    sb.Append(" ");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        [Obsolete("Weak performance in UI!")]
        public static async Task<string> MatToStringAsync(double[,] mat)
        {
            return await Task<string>.Run(() =>
            {
                long rows = mat.GetLongLength(0);
                long cols = mat.GetLongLength(1);
                StringBuilder sb = new StringBuilder();

                for (long i = 0; i < rows; ++i)
                {
                    for (long j = 0; j < cols; ++j)
                    {
                        sb.Append(mat[i, j].ToString());
                        sb.Append(" ");
                    }
                    sb.Append("\n");
                }
                return sb.ToString();
            });
        }
        /// <summary>
        /// Builds List of double arrays (Array matrix representation)
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static async Task<List<double[]>> MatToMatPresenterAsync(double[,] mat)
        {
            //Nothing complex, just ordinary async call
            return await Task.Run(() =>
            {
                long rows = mat.GetLongLength(0);
                long cols = mat.GetLongLength(1);
                var res = new List<double[]>();
                for (long i = 0; i < rows; ++i)
                {
                    double[] row = new double[cols];

                    for (long j = 0; j < cols; ++j)
                    {
                        row[j] = mat[i, j];
                    }
                    res.Add(row);
                }
                return res;
            });
        }
    }
}
