using System;
using SME;
using System.IO;
using System.Linq;
using System.Globalization;
using Deflib;

namespace Sigmoid
{       

    class MainClass{
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {


                ////// Test data r///////////////
                var control_zz = Scope.CreateBus<IndexControl>();
                
                var control_sig = Scope.CreateBus<IndexControl>();

                var zz_data = Generate_sig.gen_zz();
                var zz_flat = Deflib.Functions.Flatten(zz_data);

                var sig_expected = Deflib.Functions.Flatten(Deflib.Generate_data.sig(zz_data));


                var array_zz = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks,zz_flat);  

                var index_sig = new TestIndexSim(control_sig, 1, (int)Deflib.Parameters.num_networks);
                var index_zz = new TestIndexSim(control_zz, 1, (int)Deflib.Parameters.num_networks);
                
                var sigmoid = new Sig_Stage(control_sig, control_zz, array_zz);
                
                var outsimtra = new OutputSim(sigmoid.control_out, sigmoid.ram_out, sig_expected);

                sim

                    .BuildCSVFile()
                    .BuildGraph()
                    .Run();
            }        
        }
    


        }
    public class Sig_Stage{

        public SME.Components.SimpleDualPortMemory<double> ram_out;
        public IndexControl control_out;
        public Sig_Stage(IndexControl testcontrol, IndexControl control_in,SME.Components.SimpleDualPortMemory<double> ram_in ){

            var sig_index_A = Scope.CreateBus<IndexValue>();
            // var sig_index_B = Scope.CreateBus<IndexValue>();
            var pipeout_sig = Scope.CreateBus<IndexValue>();
            var pipeout1_sig = Scope.CreateBus<IndexValue>();
            var pipeout2_sig = Scope.CreateBus<IndexValue>();
            var pipeout3_sig = Scope.CreateBus<IndexValue>();
            var sig_result = Scope.CreateBus<ValueTransfer>();
            control_out = Scope.CreateBus<IndexControl>();


            ram_out = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks);

            var generate_Sig = new Generate(sig_index_A, ram_in.ReadControl);

            var sig_ind = new SigIndex(control_in, sig_index_A ,control_out,  testcontrol);
            var pipe_sig = new Pipe(sig_index_A, pipeout_sig);

            var sigmoid_1 = new Sigmoid_1(ram_in.ReadResult,pipeout_sig, sig_result);
            var pipe1_sig = new Pipe(pipeout_sig, pipeout1_sig);

            var sigmoid_2 = new Sigmoid_2(ram_in.ReadResult,pipeout1_sig, sig_result);
            var pipe2_sig = new Pipe(pipeout1_sig, pipeout2_sig);

            var sigmoid_3 = new Sigmoid_3(ram_in.ReadResult,pipeout2_sig, sig_result);
            var pipe3_sig = new Pipe(pipeout2_sig, pipeout3_sig);

            var toram_sig = new ToRam(sig_result, pipeout3_sig, ram_out.WriteControl);





        }
    }
}