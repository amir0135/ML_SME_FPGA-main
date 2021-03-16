using System;
using Deflib;

namespace Softplus
{      
    public class Generate_soft{


        public static double[,] gen_sla()//int[,] x)
        {   
                                
            var matmul = Deflib.Generate_data.matmul_mat(Deflib.dataMatrix.x, Deflib.dataMatrix.W0);

            var hz_hr = Deflib.Generate_data.generate_hz(matmul, Deflib.dataMatrix.x);
            
            var z_r = Deflib.Generate_data.z_r(hz_hr.Item1, hz_hr.Item2);

            var sla = Deflib.Generate_data.gen_SLA(z_r.Item1, z_r.Item2);


            // var soft = Deflib.Generate_data.soft(sla.Item2);
            
            // var soft_flat = Deflib.Functions.Flatten(soft);
        

            return sla.Item2;

        }

    }

}