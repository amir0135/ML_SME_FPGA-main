using System;
using System.Linq;
using SME;
using SME.Components;
using Deflib;

namespace Load_data
{

    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {
                var W0_control = Scope.CreateBus<IndexControl>();
                var Wr_control = Scope.CreateBus<IndexControl>();
                var Wz_control = Scope.CreateBus<IndexControl>();
                var Prelu_z_control = Scope.CreateBus<IndexControl>();
                var Prelu_r_control = Scope.CreateBus<IndexControl>();
                var z_scale_control = Scope.CreateBus<IndexControl>();
                var x_data_control = Scope.CreateBus<IndexControl>();

                var W0_data = new LoadStage(W0_control, (int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size * (int)Deflib.Parameters.input_size, "../Data/W0.csv", (int)Deflib.Parameters.input_size, (int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size);
                var Wr_data = new LoadStage(Wr_control, (int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, "../Data/Wr.csv", (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var Wz_data = new LoadStage(Wz_control, (int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, "../Data/Wz.csv", (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var Prelu_z_data = new LoadStage(Prelu_z_control, (int)Deflib.Parameters.num_networks, "../Data/prelu_z_slopes.csv", (int)Deflib.Parameters.num_networks);
                var Prelu_r_data = new LoadStage(Prelu_r_control, (int)Deflib.Parameters.num_networks, "../Data/prelu_r_slopes.csv", (int)Deflib.Parameters.num_networks);
                var z_scale_data = new LoadStage(z_scale_control, (int)Deflib.Parameters.num_networks, "../Data/z_scale.csv", 1, (int)Deflib.Parameters.num_networks);
                var x_data = new LoadStage(x_data_control, (int)Deflib.Parameters.Batchsize * (int)Deflib.Parameters.input_size, "../Data/x.csv", (int)Deflib.Parameters.Batchsize, (int)Deflib.Parameters.input_size);

                var outsim_w0 = new OutputSim(W0_data.control_out, W0_data.ram_out, W0_data.expected);
                var outsim_wr = new OutputSim(Wr_data.control_out, Wr_data.ram_out, Wr_data.expected);
                var outsim_wz = new OutputSim(Wz_data.control_out, Wz_data.ram_out, Wz_data.expected);
                var outsim_prelu_z = new OutputSim(Prelu_z_data.control_out, Prelu_z_data.ram_out, Prelu_z_data.expected);
                var outsim_Prelu_r = new OutputSim(Prelu_r_data.control_out, Prelu_r_data.ram_out, Prelu_r_data.expected);
                var outsim_z_scale = new OutputSim(z_scale_data.control_out, z_scale_data.ram_out, z_scale_data.expected);
                var outsim_x_data = new OutputSim(x_data.control_out, x_data.ram_out, x_data.expected);



                sim
                    .AddTopLevelInputs(
                        W0_control, W0_data.ram_out.WriteControl, W0_data.ram_out.ReadControl,
                        Wr_control, Wr_data.ram_out.WriteControl, Wr_data.ram_out.ReadControl,
                        Wz_control, Wz_data.ram_out.WriteControl, Wz_data.ram_out.ReadControl,
                        Prelu_z_control, Prelu_z_data.ram_out.WriteControl, Prelu_z_data.ram_out.ReadControl,
                        Prelu_r_control, Prelu_r_data.ram_out.WriteControl, Prelu_r_data.ram_out.ReadControl,
                        z_scale_control, z_scale_data.ram_out.WriteControl, z_scale_data.ram_out.ReadControl,
                        x_data_control, x_data.ram_out.WriteControl, x_data.ram_out.ReadControl
                    )
                    .AddTopLevelOutputs(
                        W0_data.control_out, W0_data.ram_out.ReadResult,
                        Wr_data.control_out, Wr_data.ram_out.ReadResult,
                        Wz_data.control_out, Wz_data.ram_out.ReadResult,
                        Prelu_z_data.control_out, Prelu_z_data.ram_out.ReadResult,
                        Prelu_r_data.control_out, Prelu_r_data.ram_out.ReadResult,
                        z_scale_data.control_out, z_scale_data.ram_out.ReadResult,
                        x_data.control_out, x_data.ram_out.ReadResult
                    )
                    .BuildCSVFile()
                    .BuildGraph()
                    .BuildVHDL()
                    .Run(exitMethod:
                        () =>
                        {
                            OutputSim[] procs =
                                {
                                    outsim_w0,
                                    outsim_wr,
                                    outsim_wz,
                                    outsim_prelu_z,
                                    outsim_Prelu_r,
                                    outsim_z_scale,
                                    outsim_x_data
                                };
                            return procs.All(x => ((IProcess)x).Finished().IsCompleted);
                        }
                        );
            }
        }
    }

    public class LoadStage
    {
        public SimpleDualPortMemory<float> ram_out;
        public IndexControl control_out;
        public float[] expected;

        public LoadStage(IndexControl control_in, int size, string CSVfile, int row)
        {
            var load_index = Scope.CreateBus<IndexValue>();
            control_out = Scope.CreateBus<IndexControl>();

            ram_out = new SimpleDualPortMemory<float>(size);

            var load_sim = new TestIndexSim(control_in, row);
            var generate_load = new Dataload(size, CSVfile, load_index, ram_out.WriteControl);
            var load_ind = new Index(control_in, load_index, control_out);

            expected = generate_load.A;
        }

        public LoadStage(IndexControl control_in, int size, string CSVfile, int row, int col)
        {
            var load_index = Scope.CreateBus<IndexValue>();
            control_out = Scope.CreateBus<IndexControl>();

            ram_out = new SimpleDualPortMemory<float>(size);

            var load_sim = new TestIndexSim(control_in, row, col);
            var generate_load = new Dataload(size, CSVfile, load_index, ram_out.WriteControl);
            var load_ind = new Index(control_in, load_index, control_out);

            expected = generate_load.A;
        }
    }

}