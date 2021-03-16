using System;
using SME;
using System.IO;
using System.Linq;
using System.Globalization;
using Deflib;

namespace Mean
{       
    class MainClass{
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {
                var control_mean = Scope.CreateBus<IndexControl>();
                
                var control_clamp = Scope.CreateBus<IndexControl>();

                var clamp_data = Generate_clamp.gen_clamp();
                var clamp_flat = Deflib.Functions.Flatten(clamp_data);

                var mean_expected = Deflib.Generate_data.ensemble_predictions(clamp_data);
                var array_clamp = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks,clamp_flat);  

                var index_clamp = new TestIndexSim(control_clamp, 1, (int)Deflib.Parameters.num_networks);
                var index_mean = new TestIndexSim(control_mean,1 ,(int)Deflib.Parameters.num_networks);
                
                var mean = new Mean_Stage(control_mean, control_clamp, array_clamp);
                
                var outsimtra = new OutputSim(mean.control_out, mean.ram_out, mean_expected);

                sim
                    .BuildCSVFile()
                    .BuildGraph()
                    .Run();
            }        
        }
    


        }
    public class Mean_Stage{

        public SME.Components.SimpleDualPortMemory<double> ram_out;
        public IndexControl control_out;

        public Mean_Stage(IndexControl testcontrol, IndexControl control_in,  SME.Components.SimpleDualPortMemory<double> ram_in ){

            var mean_index_A = Scope.CreateBus<IndexValue>();
            var mean_index_B = Scope.CreateBus<IndexValue>();

            var pipeout_mean = Scope.CreateBus<IndexValue>();
            var pipeout1_mean = Scope.CreateBus<IndexValue>();
            var mean_result = Scope.CreateBus<ValueTransfer>();
            control_out = Scope.CreateBus<IndexControl>();


            ram_out = new SME.Components.SimpleDualPortMemory<double>(1);

            var generate_mean = new Generate(mean_index_A, ram_in.ReadControl); 

            var mean_ind = new SLAIndex(control_in, mean_index_A, mean_index_B ,control_out,  testcontrol);
            // var mean_ind = new SigIndex(control_in, mean_index_A ,control_out,  testcontrol);
            var pipe_mean = new Pipe(mean_index_B, pipeout_mean);
            var mean = new Mean(ram_in.ReadResult,pipeout_mean, mean_result);
            var pipe1_mean = new Pipe(pipeout_mean, pipeout1_mean);

            var toram_mean = new ToRam(mean_result, pipeout1_mean, ram_out.WriteControl);

            

        }
    }
}