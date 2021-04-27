using System;
using SME;
using System.IO;
using System.Linq;
using System.Globalization;
using Deflib;

namespace Softplus
{

    class MainClass
    {
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

                var array_r = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks, sla_flat);
                var index_soft = new TestIndexSim(control_soft, 1, (int)Deflib.Parameters.num_networks);
                var index_r = new TestIndexSim(control_r, 1, (int)Deflib.Parameters.num_networks, 1);

                var soft = new Soft_stage(control_soft, control_r, array_r);

                var outsimtra = new OutputSim(soft.control_out, soft.ram_out, soft_expected);

                sim
                    .BuildCSVFile()
                    .BuildGraph()
                    .Run();
            }
        }
    }

    public class Soft_stage
    {
        public SME.Components.SimpleDualPortMemory<double> ram_out;
        public IndexControl control_out;

        public Soft_stage(IndexControl testcontrol, IndexControl control_in, SME.Components.SimpleDualPortMemory<double> ram_in)
        {
            var soft_index_A = Scope.CreateBus<IndexValue>();
            var flag_0 = Scope.CreateBus<Flag>();
            var flag_1 = Scope.CreateBus<Flag>();
            var flag_2 = Scope.CreateBus<Flag>();

            var pipeout0_soft = Scope.CreateBus<IndexValue>();
            var pipeout1_soft = Scope.CreateBus<IndexValue>();
            var pipeout2_soft = Scope.CreateBus<IndexValue>();

            var soft_result_0 = Scope.CreateBus<ValueTransfer>();
            var soft_result_1 = Scope.CreateBus<ValueTransfer>();
            var soft_result_2 = Scope.CreateBus<ValueTransfer>();
            control_out = Scope.CreateBus<IndexControl>();
            var pipe0_control = Scope.CreateBus<IndexControl>();
            var pipe1_control = Scope.CreateBus<IndexControl>();
            var pipe2_control = Scope.CreateBus<IndexControl>();

            // Stage 0 - Generate addresses
            var soft_ind = new SigIndex_flag(control_in, soft_index_A, pipe0_control, testcontrol, flag_0);

            // Stage 1 - Read from RAM
            var generate_soft = new Generate(soft_index_A, ram_in.ReadControl);
            var pipe_soft = new Pipe(soft_index_A, pipeout0_soft);
            var pipe_con1 = new Pipe_control(pipe0_control, pipe1_control);
            var pipe_flag1 = new Pipe_flag(flag_0, flag_1);

            //stage 2 - multiply
            var soft2 = new Softplus_1(ram_in.ReadResult, pipeout0_soft, soft_result_0, flag_1);
            var pipe_mul2 = new Pipe(pipeout0_soft, pipeout1_soft);
            var pipe_con2 = new Pipe_control(pipe1_control, pipe2_control);
            var pipe_flag2 = new Pipe_flag(flag_1, flag_2);

            // Stage 3 - subtract
            var soft1 = new Softplus_2(soft_result_0, pipeout1_soft, soft_result_1, flag_2);
            var pipe_con3 = new Pipe_control(pipe2_control, control_out);
            var should_save = new ShouldSave(pipeout1_soft, flag_2, pipeout2_soft);

            // Stage 4 - Save to RAM
            ram_out = new SME.Components.SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks);
            var toram_soft = new ToRam(soft_result_1, pipeout2_soft, ram_out.WriteControl);
        }
    }

}