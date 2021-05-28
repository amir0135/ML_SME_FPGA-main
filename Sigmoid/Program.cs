using System;
using SME;
using Deflib;

namespace Sigmoid
{

    class MainClass
    {
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

                var array_zz = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks, zz_flat);

                var index_sig = new TestIndexSim(control_sig, 1, (int)Deflib.Parameters.num_networks);
                var index_zz = new TestIndexSim(control_zz, 1, (int)Deflib.Parameters.num_networks);

                var sigmoid = new Sig_Stage(control_sig, control_zz, array_zz);

                var outsimtra = new OutputSim(sigmoid.control_out, sigmoid.ram_out, sig_expected);

                sim
                    .AddTopLevelInputs(control_zz, control_sig, array_zz.WriteControl, sigmoid.ram_out.ReadControl)
                    .AddTopLevelOutputs(sigmoid.control_out, sigmoid.ram_out.ReadResult)
                    .BuildCSVFile()
                    .BuildGraph()
                    .BuildVHDL()
                    .Run();
            }
        }
    }

    public class Sig_Stage
    {

        public SME.Components.SimpleDualPortMemory<float> ram_out;
        public IndexControl control_out;
        public Sig_Stage(IndexControl testcontrol, IndexControl control_in, SME.Components.SimpleDualPortMemory<float> ram_in)
        {
            var sig_index_A = Scope.CreateBus<IndexValue>();
            var flag_0 = Scope.CreateBus<Flag>();
            var flag_1 = Scope.CreateBus<Flag>();
            var flag_2 = Scope.CreateBus<Flag>();
            var flag_3 = Scope.CreateBus<Flag>();
            var flag_4 = Scope.CreateBus<Flag>();

            var pipeout0_sig = Scope.CreateBus<IndexValue>();
            var pipeout1_sig = Scope.CreateBus<IndexValue>();
            var pipeout2_sig = Scope.CreateBus<IndexValue>();
            var pipeout3_sig = Scope.CreateBus<IndexValue>();
            var pipeout4_sig = Scope.CreateBus<IndexValue>();

            var sig_result_0 = Scope.CreateBus<ValueTransfer>();
            var sig_result_1 = Scope.CreateBus<ValueTransfer>();
            var sig_result_2 = Scope.CreateBus<ValueTransfer>();
            var sig_result_3 = Scope.CreateBus<ValueTransfer>();
            control_out = Scope.CreateBus<IndexControl>();
            var pipe0_control = Scope.CreateBus<IndexControl>();
            var pipe1_control = Scope.CreateBus<IndexControl>();
            var pipe2_control = Scope.CreateBus<IndexControl>();
            var pipe3_control = Scope.CreateBus<IndexControl>();
            var pipe4_control = Scope.CreateBus<IndexControl>();

            // Stage 0 - Generate addresses
            var sig_ind = new SigIndex_flag(control_in, sig_index_A, pipe0_control, testcontrol, flag_0);

            // Stage 1 - Read from RAM
            var generate_sig = new Generate(sig_index_A, ram_in.ReadControl);
            var pipe_sig = new Pipe(sig_index_A, pipeout0_sig);
            var pipe_con1 = new Pipe_control(pipe0_control, pipe1_control);
            var pipe_flag1 = new Pipe_flag(flag_0, flag_1);

            //stage 2 - exponentiel
            var sig1 = new Sigmoid_1(ram_in.ReadResult, pipeout0_sig, sig_result_0, flag_1);
            var pipe_mul2 = new Pipe(pipeout0_sig, pipeout1_sig);
            var pipe_con2 = new Pipe_control(pipe1_control, pipe2_control);
            var pipe_flag2 = new Pipe_flag(flag_1, flag_2);

            // Stage 3 - add
            var sig2 = new Sigmoid_2(sig_result_0, pipeout1_sig, sig_result_1, flag_2);
            var pipe_mul3 = new Pipe(pipeout1_sig, pipeout2_sig);
            var pipe_con3 = new Pipe_control(pipe2_control, pipe3_control);
            var pipe_flag3 = new Pipe_flag(flag_2, flag_3);

            // Stage 4 - divide
            var sig3 = new Sigmoid_3(sig_result_1, pipeout2_sig, sig_result_2, flag_3);
            var pipe_con4 = new Pipe_control(pipe3_control, control_out);
            var should_save = new ShouldSave(pipeout2_sig, flag_3, pipeout3_sig);

            // Stage 4 - Save to RAM
            ram_out = new SME.Components.SimpleDualPortMemory<float>((int)Deflib.Parameters.num_networks);
            var toram_sig = new ToRam(sig_result_2, pipeout3_sig, ram_out.WriteControl);
        }
    }

}