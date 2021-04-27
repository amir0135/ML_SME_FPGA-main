using System;
using Deflib;

namespace sum_lastaxis
{

    public class Generate_z_r
    {
        public static Tuple<double[,,], double[,,]> gen_z_r()
        {
            var matmul = Deflib.Generate_data.matmul_mat(Deflib.dataMatrix.x, Deflib.dataMatrix.W0);
            var hz_hr = Deflib.Generate_data.generate_hz(matmul, Deflib.dataMatrix.x);
            var z_r = Deflib.Generate_data.z_r(hz_hr.Item1, hz_hr.Item2);

            return new Tuple<double[,,], double[,,]>(z_r.Item1, z_r.Item2);
        }
    }

}