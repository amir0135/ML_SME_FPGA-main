using System;
using Deflib;

namespace Mean
{

    public class Generate_clamp
    {
        public static float[,] gen_clamp()
        {
            //making softplus function
            var matmul = Deflib.Generate_data.matmul_mat(Deflib.dataMatrix.x, Deflib.dataMatrix.W0);
            var hz_hr = Deflib.Generate_data.generate_hz(matmul, Deflib.dataMatrix.x);
            var z_r = Deflib.Generate_data.z_r(hz_hr.Item1, hz_hr.Item2);
            var sla = Deflib.Generate_data.gen_SLA(z_r.Item1, z_r.Item2);
            var sla_flat = Deflib.Functions.Flatten(sla.Item2);
            var soft = Deflib.Generate_data.soft(sla.Item2);

            // making mulmin
            var zz = Deflib.Generate_data.zz(sla.Item1);
            var sig = Deflib.Generate_data.sig(zz);
            var mulmin = Deflib.Generate_data.mulmin(sig);

            //making rz
            var rz = Deflib.Generate_data.rz(soft, mulmin);

            //making clamp
            var clamp = Deflib.Generate_data.clamp(rz);

            return clamp;
        }
    }

}