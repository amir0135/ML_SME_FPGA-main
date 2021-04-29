using System;
using Deflib;

namespace z_r
{

    public class Generate_hz
    {
        public static Tuple<float[,,], float[,,]> generate_hz()
        {
            var matmul = Deflib.Generate_data.matmul_mat(Deflib.dataMatrix.x, Deflib.dataMatrix.W0);
            var hz = Deflib.Generate_data.generate_hz(matmul, Deflib.dataMatrix.x);

            return new Tuple<float[,,], float[,,]>(hz.Item1, hz.Item2);
        }
    }

}