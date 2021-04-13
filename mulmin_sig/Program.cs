using System;
using SME;
using System.IO;
using System.Linq;
using System.Globalization;
using Deflib;

namespace mulmin_sig
{       

    class MainClass{
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {


                ////// Test data r///////////////
                var control_sig = Scope.CreateBus<IndexControl>();
                
                var control_mulmin = Scope.CreateBus<IndexControl>();

                var sig_data = Generate_mulmin.gen_sig();
                var sig_flat = Deflib.Functions.Flatten(sig_data);

                var mulmin_expected = Deflib.Functions.Flatten(Deflib.Generate_data.mulmin(sig_data));

                var array_sigmoid = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks,sig_flat);  

                var index_mulmin = new TestIndexSim(control_mulmin, 1, (int)Deflib.Parameters.num_networks);
                var index_sig = new TestIndexSim(control_sig, 1, (int)Deflib.Parameters.num_networks);
                
                var mulmin = new minmul_Stage(control_mulmin, control_sig, array_sigmoid);
                
                var outsimtra = new OutputSim(mulmin.control_out, mulmin.ram_out, mulmin_expected);

                sim

                    .BuildCSVFile()
                    .BuildGraph()
                    .Run();
            }        
        }
    


        }
    public class minmul_Stage{

        public SME.Components.SimpleDualPortMemory<double> ram_out;
        public IndexControl control_out;
        public minmul_Stage(IndexControl testcontrol, IndexControl control_in, SME.Components.SimpleDualPortMemory<double> ram_in ){


            var mulmin_index_A = Scope.CreateBus<IndexValue>();

            var pipeout_mulmin = Scope.CreateBus<IndexValue>();
            var pipeout1_mulmin = Scope.CreateBus<IndexValue>();
            var pipeout2_mulmin = Scope.CreateBus<IndexValue>();
            var mulmin_result = Scope.CreateBus<ValueTransfer>();
            control_out = Scope.CreateBus<IndexControl>();


            ram_out = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks);

            var generate_mulmin = new Generate(mulmin_index_A, ram_in.ReadControl);

            var mulmin_ind = new SigIndex(control_in, mulmin_index_A ,control_out,  testcontrol);
            var mulmin2 = new MulMin_2(ram_in.ReadResult,pipeout_mulmin, mulmin_result);
            var pipe_mulmin = new Pipe(mulmin_index_A, pipeout_mulmin);

            // ikke sikker på hvordan jeg hente mulmin result in når det er en bus. og skal det pipes i mulmin2 før jeg er helt 
            // færdig med udregning?
            var mulmin1 = new MulMin_1(ram_in.ReadResult,pipeout_mulmin, mulmin_result);
            var pipe1_mulmin = new Pipe(pipeout_mulmin, pipeout1_mulmin);

            var toram_mulmin = new ToRam(mulmin_result, pipeout1_mulmin, ram_out.WriteControl);
        }
    }
}