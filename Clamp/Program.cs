using System;
using SME;
using System.IO;
using System.Linq;
using System.Globalization;
using Deflib;

namespace Clamp
{       
    class MainClass{
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {


                ////// Test data r///////////////
                var control_clamp = Scope.CreateBus<IndexControl>();
                
                var control_rz = Scope.CreateBus<IndexControl>();

                var rz_data = Generate_RZ.gen_RZ();
                var rz_flat = Deflib.Functions.Flatten(rz_data);

                var clamp_expected = Deflib.Functions.Flatten(Deflib.Generate_data.clamp(rz_data));

                var array_rz = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks,rz_flat);  

                var index_rz = new TestIndexSim(control_rz, 1, (int)Deflib.Parameters.num_networks);
                var index_clamp = new TestIndexSim(control_clamp, 1, (int)Deflib.Parameters.num_networks);
                
                var clamp = new Clamp_Stage(control_clamp, control_rz, array_rz, -(int)Deflib.Parameters.max_predict, (int)Deflib.Parameters.max_predict);
                
                var outsimtra = new OutputSim(clamp.control_out, clamp.ram_out, clamp_expected);

                sim
                    .BuildCSVFile()
                    .BuildGraph()
                    .Run();
            }        
        }
    


        }
    public class Clamp_Stage{

        public SME.Components.SimpleDualPortMemory<double> ram_out;
        public IndexControl control_out;

        int min; int max;
        public Clamp_Stage(IndexControl testcontrol, IndexControl control_in,SME.Components.SimpleDualPortMemory<double> ram_in, int min, int max){

            var clamp_index = Scope.CreateBus<IndexValue>();
            var pipeout_clamp = Scope.CreateBus<IndexValue>();
            var pipeout1_clamp = Scope.CreateBus<IndexValue>();
            var clamp_result = Scope.CreateBus<ValueTransfer>();

            control_out = Scope.CreateBus<IndexControl>();

            ram_out = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks);

            var generate_clamp = new Generate(clamp_index, ram_in.ReadControl);
            
            var clampind = new SigIndex(control_in, clamp_index, control_out, testcontrol);
            var pipe_clamp = new Pipe(clamp_index, pipeout_clamp);
            var clamp = new Clamp(ram_in.ReadResult, pipeout_clamp, clamp_result, min, max);
            var pipe1_clamp  = new Pipe(pipeout_clamp, pipeout1_clamp);
            
            var toram_clamp = new ToRam(clamp_result, pipeout1_clamp, ram_out.WriteControl);
            


        }
    }
}