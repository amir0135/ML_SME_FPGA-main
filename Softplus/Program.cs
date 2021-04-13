using System;
using SME;
using System.IO;
using System.Linq;
using System.Globalization;
using Deflib;

namespace Softplus
{       

    class MainClass{
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {


                ////// Test data r///////////////
                var control_soft = Scope.CreateBus<IndexControl>();
                
                var control_r = Scope.CreateBus<IndexControl>();

                var sla_r_data = Generate_soft.gen_sla();
                var sla_flat = Deflib.Functions.Flatten(sla_r_data);

                var soft_expected = Deflib.Functions.Flatten(Deflib.Generate_data.soft(sla_r_data));

                var array_r = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks,sla_flat);  
                var index_soft = new TestIndexSim(control_soft, 1, (int)Deflib.Parameters.num_networks);
                var index_r = new TestIndexSim(control_r, 1, (int)Deflib.Parameters.num_networks, 1);
                
                var soft = new Soft_stage(control_soft,control_r, array_r);
                
                var outsimtra = new OutputSim(soft.control_out, soft.ram_out, soft_expected);

                sim

                    .BuildCSVFile()
                    .BuildGraph()
                    .Run();
            }        
        }
    


        }
    public class Soft_stage{

        public SME.Components.SimpleDualPortMemory<double> ram_out;
        public IndexControl control_out;
        public Soft_stage(IndexControl testcontrol, IndexControl control_in,SME.Components.SimpleDualPortMemory<double> ram_in ){

            var soft_index = Scope.CreateBus<IndexValue>();
            var pipeout_soft = Scope.CreateBus<IndexValue>();
            var pipeout1_soft = Scope.CreateBus<IndexValue>();
            var pipeout2_soft = Scope.CreateBus<IndexValue>();
            var soft_result = Scope.CreateBus<ValueTransfer>();
            
            control_out = Scope.CreateBus<IndexControl>();
            
            ram_out = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks);

            var generate_soft = new Generate(soft_index, ram_in.ReadControl);

            var soft_ind = new SigIndex(control_in, soft_index ,control_out,  testcontrol);
            var pipe_soft = new Pipe(soft_index, pipeout_soft);
            var softplus = new Softplus_1(ram_in.ReadResult,pipeout_soft, soft_result);
            var pipe1_soft = new Pipe(pipeout_soft, pipeout1_soft );

            var softplus_2 = new Softplus_2(ram_in.ReadResult,pipeout1_soft, soft_result);
            var pipe2_soft = new Pipe(pipeout1_soft, pipeout2_soft );

            var toram_soft = new ToRam(soft_result, pipeout2_soft, ram_out.WriteControl);
        }
    }
}

