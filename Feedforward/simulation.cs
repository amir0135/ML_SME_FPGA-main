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
        public static float[] tra_W0_data = Generate_transpose.transpose();
        public static float[,] matmul_data = Generate_matmul.Matmul_generate();
        public static Tuple<float[,,], float[,,]> hz_data = Generate_hz.generate_hz();
        public static Tuple<float[,,], float[,,]> hr_data = Generate_hz.generate_hz();
        public static Tuple<float[,,], float[,,]> z_data = Generate_z_r.gen_z_r();
        public static Tuple<float[,,], float[,,]> r_data = Generate_z_r.gen_z_r();
        public static Tuple<float[,], float[,]> SLA_data = Generate_sla.gen_sla();
        public static float[,] zz_data = Generate_sig.gen_zz();
        public static float[,] sig_data = Generate_mulmin.gen_sig();
        public static float[,] sla_r_data = Generate_soft.gen_sla();
        public static float[,] soft_data = Generate_rz.gen_soft_mulmin().Item1;
        public static float[,] rz_data = Generate_RZ.gen_RZ();
        public static float[,] clamp_data = Generate_clamp.gen_clamp();

        public static float[] transpose_expected = Deflib.Functions.Flatten(Transpose.Generate_data.transpose());
        public static float[] matmul_expected = Deflib.Functions.Flatten(Deflib.Generate_data.matmul_mat(Deflib.dataMatrix.x, Deflib.dataMatrix.W0));
        public static float[] hz_expected = Deflib.Functions.Flatten(Deflib.Generate_data.generate_hz(matmul_data, Deflib.dataMatrix.x).Item1);
        public static float[] hr_expected = Deflib.Functions.Flatten(Deflib.Generate_data.generate_hz(matmul_data, Deflib.dataMatrix.x).Item2);
        public static float[] z_expected = Deflib.Functions.Flatten(Deflib.Generate_data.z_r(hz_data.Item1, hz_data.Item2).Item1);
        public static float[] r_expected = Deflib.Functions.Flatten(Deflib.Generate_data.z_r(hr_data.Item1, hr_data.Item2).Item2);
        public static float[] SLA_z_expected = Deflib.Functions.Flatten(Deflib.Generate_data.gen_SLA(z_data.Item1,z_data.Item2).Item1);
        public static float[] SLA_r_expected = Deflib.Functions.Flatten(Deflib.Generate_data.gen_SLA(r_data.Item1,r_data.Item2).Item2);
        public static float[] zz_expected = Deflib.Functions.Flatten(Deflib.Generate_data.zz(SLA_data.Item1));
        public static float[] sig_expected = Deflib.Functions.Flatten(Deflib.Generate_data.sig(zz_data));
        public static float[] mulmin_expected = Deflib.Functions.Flatten(Deflib.Generate_data.mulmin(sig_data));
        public static float[] soft_expected = Deflib.Functions.Flatten(Deflib.Generate_data.soft(sla_r_data));
        public static float[] RZ_expected = Deflib.Functions.Flatten(Deflib.Generate_data.rz(soft_data, sig_data));
        public static float[] clamp_expected = Deflib.Functions.Flatten(Deflib.Generate_data.clamp(rz_data));
        public static float[] mean_expected = Deflib.Generate_data.ensemble_predictions(clamp_data);
    }

}
