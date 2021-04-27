using System;
using Deflib;

namespace HzHr
{

    public class Generate_matmul
    {
        public static double[,] Matmul_generate()
        {
            return Deflib.Generate_data.matmul_mat(Deflib.dataMatrix.x, Deflib.dataMatrix.W0);
        }
    }

}