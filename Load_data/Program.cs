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
                var W0_data = new LoadStage((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size * (int)Deflib.Parameters.input_size, "../Data/W0.csv", (int)Deflib.Parameters.input_size, (int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size);
                var Wr_data = new LoadStage((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, "../Data/Wr.csv", (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var Wz_data = new LoadStage((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, "../Data/Wz.csv", (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var Prelu_z_data = new LoadStage((int)Deflib.Parameters.num_networks, "../Data/prelu_z_slopes.csv", (int)Deflib.Parameters.num_networks);
                var Prelu_r_data = new LoadStage((int)Deflib.Parameters.num_networks, "../Data/prelu_r_slopes.csv", (int)Deflib.Parameters.num_networks);
                var z_scale_data = new LoadStage((int)Deflib.Parameters.num_networks, "../Data/z_scale.csv", 1, (int)Deflib.Parameters.num_networks);
                var x_data = new LoadStage((int)Deflib.Parameters.Batchsize * (int)Deflib.Parameters.input_size, "../Data/x.csv", (int)Deflib.Parameters.Batchsize, (int)Deflib.Parameters.input_size);

                var outsim_w0 = new OutputSim(W0_data.control, W0_data.output, W0_data.expected);
                var outsim_wr = new OutputSim(Wr_data.control, Wr_data.output, Wr_data.expected);
                var outsim_wz = new OutputSim(Wz_data.control, Wz_data.output, Wz_data.expected);
                var outsim_prelu_z = new OutputSim(Prelu_z_data.control, Prelu_z_data.output, Prelu_z_data.expected);
                var outsim_Prelu_r = new OutputSim(Prelu_r_data.control, Prelu_r_data.output, Prelu_r_data.expected);
                var outsim_z_scale = new OutputSim(z_scale_data.control, z_scale_data.output, z_scale_data.expected);
                var outsim_x_data = new OutputSim(x_data.control, x_data.output, x_data.expected);



                sim
                    .BuildCSVFile()
                    .BuildGraph()
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
        public SimpleDualPortMemory<float> output;
        public IndexControl control;
        public float[] expected;

        public LoadStage(int size, string CSVfile, int row)
        {
            var load_control = Scope.CreateBus<IndexControl>();
            var load_index = Scope.CreateBus<IndexValue>();
            control = Scope.CreateBus<IndexControl>();

            output = new SimpleDualPortMemory<float>(size);

            var load_sim = new TestIndexSim(load_control, row);
            var generate_load = new Dataload(size, CSVfile, load_index, output.WriteControl);
            var load_ind = new Index(load_control, load_index, control);

            expected = generate_load.A;
        }

        public LoadStage(int size, string CSVfile, int row, int col)
        {
            var load_control = Scope.CreateBus<IndexControl>();
            var load_index = Scope.CreateBus<IndexValue>();
            control = Scope.CreateBus<IndexControl>();

            output = new SimpleDualPortMemory<float>(size);

            var load_sim = new TestIndexSim(load_control, row, col);
            var generate_load = new Dataload(size, CSVfile, load_index, output.WriteControl);
            var load_ind = new Index(load_control, load_index, control);

            expected = generate_load.A;
        }
    }

}