using System;
using SME;
using Deflib;

namespace mulmin_sig
{

    class MainClass
    {
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

                var array_sigmoid = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks, sig_flat);

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

    public class minmul_Stage
    {
        public SME.Components.SimpleDualPortMemory<double> ram_out;
        public IndexControl control_out;

        public minmul_Stage(IndexControl testcontrol, IndexControl control_in, SME.Components.SimpleDualPortMemory<double> ram_in)
        {
            var mulmin_index_A = Scope.CreateBus<IndexValue>();
            var flag_0 = Scope.CreateBus<Flag>();
            var flag_1 = Scope.CreateBus<Flag>();
            var flag_2 = Scope.CreateBus<Flag>();

            var pipeout0_mulmin = Scope.CreateBus<IndexValue>();
            var pipeout1_mulmin = Scope.CreateBus<IndexValue>();
            var pipeout2_mulmin = Scope.CreateBus<IndexValue>();

            var mulmin_result_0 = Scope.CreateBus<ValueTransfer>();
            var mulmin_result_1 = Scope.CreateBus<ValueTransfer>();
            var mulmin_result_2 = Scope.CreateBus<ValueTransfer>();
            control_out = Scope.CreateBus<IndexControl>();
            var pipe0_control = Scope.CreateBus<IndexControl>();
            var pipe1_control = Scope.CreateBus<IndexControl>();
            var pipe2_control = Scope.CreateBus<IndexControl>();

            // Stage 0 - Generate addresses
            var mulmin_ind = new SigIndex_flag(control_in, mulmin_index_A, pipe0_control, testcontrol, flag_0);

            // Stage 1 - Read from RAM
            var generate_mulmin = new Generate(mulmin_index_A, ram_in.ReadControl);
            var pipe_mulmin = new Pipe(mulmin_index_A, pipeout0_mulmin);
            var pipe_con1 = new Pipe_control(pipe0_control, pipe1_control);
            var pipe_flag1 = new Pipe_flag(flag_0, flag_1);

            //stage 2 - multiply
            var mulmin2 = new Mulmin_mul(ram_in.ReadResult, pipeout0_mulmin, mulmin_result_0, flag_1);
            var pipe_mul2 = new Pipe(pipeout0_mulmin, pipeout1_mulmin);
            var pipe_con2 = new Pipe_control(pipe1_control, pipe2_control);
            var pipe_flag2 = new Pipe_flag(flag_1, flag_2);

            // Stage 3 - subtract
            var mulmin1 = new Mulmin_sub(mulmin_result_0, pipeout1_mulmin, mulmin_result_1, flag_2);
            var pipe_con3 = new Pipe_control(pipe2_control, control_out);
            var should_save = new ShouldSave(pipeout1_mulmin, flag_2, pipeout2_mulmin);

            // Stage 4 - Save to RAM
            ram_out = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks);
            var toram_mulmin = new ToRam(mulmin_result_1, pipeout2_mulmin, ram_out.WriteControl);
        }
    }

}