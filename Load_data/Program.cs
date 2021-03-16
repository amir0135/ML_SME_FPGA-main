using System;
using SME;
using SME.Components;
using System.Linq;
using Deflib;
namespace Load_data
{       

    class MainClass{
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {

                var W0_data = new LoadStage((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size * (int)Deflib.Parameters.input_size, "../Data/W0.csv",(int)Deflib.Parameters.input_size, (int)Deflib.Parameters.num_networks*(int)Deflib.Parameters.hidden_size);
                var Wr_data = new LoadStage((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, "../Data/Wr.csv", (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var Wz_data = new LoadStage((int)Deflib.Parameters.num_networks*(int)Deflib.Parameters.hidden_size, "../Data/Wz.csv", (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size);
                var Prelu_z_data = new LoadStage((int)Deflib.Parameters.num_networks, "../Data/prelu_z_slopes.csv",(int)Deflib.Parameters.num_networks);
                var Prelu_r_data = new LoadStage((int)Deflib.Parameters.num_networks, "../Data/prelu_r_slopes.csv",(int)Deflib.Parameters.num_networks);
                var z_scale_data = new LoadStage((int)Deflib.Parameters.num_networks, "../Data/z_scale.csv", 1, (int)Deflib.Parameters.num_networks);
                var x_data = new LoadStage((int)Deflib.Parameters.Batchsize*(int)Deflib.Parameters.input_size, "../Data/x.csv", (int)Deflib.Parameters.Batchsize, (int)Deflib.Parameters.input_size);


                var outsimtra = new OutputSim_T(Prelu_z_data.control, Prelu_z_data.output);
       

                sim

                    .BuildCSVFile()
                    .BuildGraph()
                    .Run();
            }        
        }
    }

    public class LoadStage{

        public SME.Components.SimpleDualPortMemory<double> output;
        public IndexControl control;
        

        public LoadStage(int size, string CSVfile, int row){


            var load_control = Scope.CreateBus<IndexControl>();
            var load_index = Scope.CreateBus<IndexValue>();
            control = Scope.CreateBus<IndexControl>();

            
            output = new SME.Components.SimpleDualPortMemory<double>(size);

            var load_sim = new TestIndexSim(load_control, row);
            var generate_load = new Dataload(size, CSVfile, load_index, output.WriteControl);
            var load_ind = new Index(load_control, load_index, control);

        }

        public LoadStage(int size, string CSVfile, int row, int col){


            var load_control = Scope.CreateBus<IndexControl>();
            var load_index = Scope.CreateBus<IndexValue>();
            control = Scope.CreateBus<IndexControl>();

            
            output = new SME.Components.SimpleDualPortMemory<double>(size);

            var load_sim = new TestIndexSim(load_control, row, col);
            var generate_load = new Dataload(size, CSVfile, load_index, output.WriteControl);
            var load_ind = new Index(load_control, load_index, control);

        }

    }
}