using System;
using Deflib;
using Transpose;
using Matmul;
using Load_data;
using HzHr;
using z_r;
using sum_lastaxis;
using zz;
using Sigmoid;
using mulmin_sig;
using Softplus;
using RZ;
using Clamp;
using Mean;

namespace Feedforward
{

    public static class Expec_val
    {
        public static double[] tra_W0_data = Generate_transpose.transpose();
        public static double[,] matmul_data = Generate_matmul.Matmul_generate();
        public static Tuple<double[,,], double[,,]> hz_data = Generate_hz.generate_hz();
        public static Tuple<double[,,], double[,,]> z_data = Generate_z_r.gen_z_r();
        public static Tuple<double[,,], double[,,]> r_data = Generate_z_r.gen_z_r();
        public static Tuple<double[,], double[,]> SLA_data = Generate_sla.gen_sla();
        public static double[,] zz_data = Generate_sig.gen_zz();
        public static double[,] sig_data = Generate_mulmin.gen_sig();
        public static double[,] sla_r_data = Generate_soft.gen_sla();
        public static double[,] soft_data = Generate_rz.gen_soft_mulmin().Item1;
        public static double[,] rz_data = Generate_RZ.gen_RZ();
        public static double[,] clamp_data = Generate_clamp.gen_clamp();

        public static double[] transpose_expected = Deflib.Functions.Flatten(Transpose.Generate_data.transpose());
        public static double[] matmul_expected = Deflib.Functions.Flatten(Deflib.Generate_data.matmul_mat(Deflib.dataMatrix.x, Deflib.dataMatrix.W0));
        public static double[] hz_expected = Deflib.Functions.Flatten(Deflib.Generate_data.generate_hz(matmul_data, Deflib.dataMatrix.x).Item1);
        public static double[] z_expected = Deflib.Functions.Flatten(Deflib.Generate_data.z_r(hz_data.Item1, hz_data.Item2).Item1);
        public static double[] SLA_z_expected = Deflib.Functions.Flatten(Deflib.Generate_data.gen_SLA(z_data.Item1,z_data.Item2).Item1);
        public static double[] SLA_r_expected = Deflib.Functions.Flatten(Deflib.Generate_data.gen_SLA(r_data.Item1,r_data.Item2).Item2);
        public static double[] zz_expected = Deflib.Functions.Flatten(Deflib.Generate_data.zz(SLA_data.Item1));
        public static double[] sig_expected = Deflib.Functions.Flatten(Deflib.Generate_data.sig(zz_data));
        public static double[] mulmin_expected = Deflib.Functions.Flatten(Deflib.Generate_data.mulmin(sig_data));
        public static double[] soft_expected = Deflib.Functions.Flatten(Deflib.Generate_data.soft(sla_r_data));
        public static double[] RZ_expected = Deflib.Functions.Flatten(Deflib.Generate_data.rz(soft_data, sig_data));
        public static double[] clamp_expected = Deflib.Functions.Flatten(Deflib.Generate_data.clamp(rz_data));
        public static double[] mean_expected = Deflib.Generate_data.ensemble_predictions(clamp_data);
    }

}
