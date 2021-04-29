using System;
using Deflib;

namespace Sigmoid
{

    public class Generate_sig
    {
        public static float[,] gen_zz()
        {
            var matmul = Deflib.Generate_data.matmul_mat(Deflib.dataMatrix.x, Deflib.dataMatrix.W0);
            var hz_hr = Deflib.Generate_data.generate_hz(matmul, Deflib.dataMatrix.x);
            var z_r = Deflib.Generate_data.z_r(hz_hr.Item1, hz_hr.Item2);
            var sla = Deflib.Generate_data.gen_SLA(z_r.Item1, z_r.Item2);
            var zz = Deflib.Generate_data.zz(sla.Item1);

            return zz;
        }
    }

}