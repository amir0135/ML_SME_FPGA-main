using System;
using System.Linq;
using SME;
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

    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {
                /////// data //////////
                var W0_data = new LoadStage((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size * (int)Deflib.Parameters.input_size, "../Data/W0.csv",(int)Deflib.Parameters.input_size, (int)Deflib.Parameters.num_networks*(int)Deflib.Parameters.hidden_size);
                var Wr_data = new LoadStage((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, "../Data/Wr.csv", (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var Wz_data = new LoadStage((int)Deflib.Parameters.num_networks*(int)Deflib.Parameters.hidden_size, "../Data/Wz.csv", (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var Prelu_z_data = new LoadStage((int)Deflib.Parameters.num_networks, "../Data/prelu_z_slopes.csv",(int)Deflib.Parameters.num_networks);
                var Prelu_r_data = new LoadStage((int)Deflib.Parameters.num_networks, "../Data/prelu_r_slopes.csv",(int)Deflib.Parameters.num_networks);
                var z_scale_data = new LoadStage((int)Deflib.Parameters.num_networks, "../Data/z_scale.csv", 1, (int)Deflib.Parameters.num_networks);
                var x_data = new LoadStage((int)Deflib.Parameters.Batchsize*(int)Deflib.Parameters.input_size, "../Data/x.csv", (int)Deflib.Parameters.Batchsize, (int)Deflib.Parameters.input_size);

                ////// SME ///////////////
                var transpose = new TransposeStage(W0_data.control, W0_data.output);

                var matmul = new MatmulStage(x_data.control, transpose.control_out, x_data.output, transpose.ram_out);

                var control_Hz = Scope.CreateBus<IndexControl>();
                var index_hz = new TestIndexSim(control_Hz, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size, 1);
                var hz = new HzStage(control_Hz, matmul.control_out, Prelu_z_data.control, matmul.ram_out_1, Prelu_z_data.output);

                var control_hr = Scope.CreateBus<IndexControl>();
                var index_hr = new TestIndexSim(control_hr, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size, 1);
                var hr = new HzStage(control_hr, matmul.control_out, Prelu_r_data.control, matmul.ram_out_1, Prelu_r_data.output);

                var control_z= Scope.CreateBus<IndexControl>();
                var index_z= new TestIndexSim(control_z, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var z = new zStage(control_z, hz.control_out, Wz_data.control, hz.ram_out, Wz_data.output);

                var control_r= Scope.CreateBus<IndexControl>();
                var index_r= new TestIndexSim(control_r, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var r = new zStage(control_r, hr.control_out, Wr_data.control, hr.ram_out, Wr_data.output);

                var control_SLA_z = Scope.CreateBus<IndexControl>();
                var index_SLA_z = new TestIndexSim(control_SLA_z, (int)Deflib.Parameters.num_networks,  (int)Deflib.Parameters.hidden_size);
                var SLA_z = new SLA_Stage(control_SLA_z, z.control_out, z.ram_out);

                var control_SLA_r = Scope.CreateBus<IndexControl>();
                var index_SLA_r = new TestIndexSim(control_SLA_r, (int)Deflib.Parameters.num_networks,  (int)Deflib.Parameters.hidden_size);
                var SLA_r = new SLA_Stage(control_SLA_r, r.control_out, r.ram_out);

                var control_zz = Scope.CreateBus<IndexControl>();
                var index_zz = new TestIndexSim(control_zz, 1, (int)Deflib.Parameters.num_networks);
                var zz = new zz_Stage(control_zz, z_scale_data.control, SLA_z.control_out, z_scale_data.output, SLA_z.ram_out);

                var control_sig = Scope.CreateBus<IndexControl>();
                var index_sig = new TestIndexSim(control_sig, 1, (int)Deflib.Parameters.num_networks);
                var sigmoid = new Sig_Stage(control_sig, zz.control_out, zz.ram_out);

                var control_mulmin = Scope.CreateBus<IndexControl>();
                var index_mulmin = new TestIndexSim(control_mulmin, 1, (int)Deflib.Parameters.num_networks);
                var mulmin = new minmul_Stage(control_mulmin, sigmoid.control_out, sigmoid.ram_out);

                var control_soft = Scope.CreateBus<IndexControl>();
                var index_soft = new TestIndexSim(control_soft, 1, (int)Deflib.Parameters.num_networks);
                var soft = new Soft_stage(control_soft, SLA_r.control_out, SLA_r.ram_out);

                var control_rz = Scope.CreateBus<IndexControl>();
                var index_rz = new TestIndexSim(control_rz, 1, (int)Deflib.Parameters.num_networks);
                var rz = new RZStage(control_rz, sigmoid.control_out, soft.control_out, sigmoid.ram_out, soft.ram_out);
                
                var control_clamp = Scope.CreateBus<IndexControl>();
                var index_clamp = new TestIndexSim(control_clamp, 1, (int)Deflib.Parameters.num_networks);
                var clamp = new Clamp_Stage(control_clamp, rz.control_out, rz.ram_out, -(int)Deflib.Parameters.max_predict, (int)Deflib.Parameters.max_predict);

                var control_mean = Scope.CreateBus<IndexControl>();
                var index_mean = new TestIndexSim(control_mean,1 ,(int)Deflib.Parameters.num_networks);
                var mean = new Mean_Stage(control_mean, clamp.control_out, clamp.ram_out);

                var outsim_transpose = new OutputSim(transpose.control_out, transpose.ram_out, Expec_val.transpose_expected);
                var outsim_matmul = new OutputSim(matmul.control_out, matmul.ram_out_1, Expec_val.matmul_expected);
                var outsim_hz = new OutputSim(hz.control_out, hz.ram_out, Expec_val.hz_expected);
                var outsim_z = new OutputSim(z.control_out, z.ram_out, Expec_val.z_expected);
                var outsim_r = new OutputSim(r.control_out, r.ram_out, Expec_val.r_expected);
                var outsim_slaz = new OutputSim(SLA_z.control_out, SLA_z.ram_out, Expec_val.SLA_z_expected);
                var outsim_slar = new OutputSim(SLA_r.control_out, SLA_r.ram_out, Expec_val.SLA_r_expected);
                var outsim_zz = new OutputSim(zz.control_out, zz.ram_out, Expec_val.zz_expected);
                var outsim_sig = new OutputSim(sigmoid.control_out, sigmoid.ram_out,  Expec_val.sig_expected);
                var outsim_mulmin = new OutputSim(mulmin.control_out, mulmin.ram_out, Expec_val.mulmin_expected);
                var outsim_soft = new OutputSim(soft.control_out, soft.ram_out, Expec_val.soft_expected);
                var outsim_rz = new OutputSim(rz.control_out, rz.ram_out, Expec_val.RZ_expected);
                var outsim_clamp = new OutputSim(clamp.control_out, clamp.ram_out, Expec_val.clamp_expected);
                var outsim_mean = new OutputSim(mean.control_out, mean.ram_out, Expec_val.mean_expected);

                uint ticks = 0;

                sim
                    .AddTicker(_ => ticks++)
                    .BuildCSVFile()
                    .BuildGraph(render_buses:false)
                    .Run(exitMethod: () =>
                        {
                            OutputSim[] procs =
                            {
                                outsim_transpose,
                                outsim_matmul,
                                outsim_hz,
                                outsim_z,
                                outsim_r,
                                outsim_slaz,
                                outsim_slar,
                                outsim_zz,
                                outsim_sig,
                                outsim_mulmin,
                                outsim_soft,
                                outsim_rz,
                                outsim_clamp,
                                outsim_mean
                            };
                            return procs.All(x => ((IProcess)x).Finished().IsCompleted);
                        }
                    );

                Console.WriteLine(ticks);
            }
        }
    }
}

