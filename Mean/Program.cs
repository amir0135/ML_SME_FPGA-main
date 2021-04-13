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

        public Mean_Stage(IndexControl testcontrol, IndexControl control_in, SME.Components.SimpleDualPortMemory<double> ram_in) {

            var mean_index_A = Scope.CreateBus<IndexValue>();
            var mean_index_B = Scope.CreateBus<IndexValue>();
            var flag_0 = Scope.CreateBus<Flag>();
            var flag_1 = Scope.CreateBus<Flag>();
            var flag_2 = Scope.CreateBus<Flag>();

            var pipeout0_mean = Scope.CreateBus<IndexValue>();
            var pipeout1_mean = Scope.CreateBus<IndexValue>();
            var pipeout2_mean = Scope.CreateBus<IndexValue>();
            var mean_result_0 = Scope.CreateBus<ValueTransfer>();
            var mean_result_1 = Scope.CreateBus<ValueTransfer>();
            var mean_result_2 = Scope.CreateBus<ValueTransfer>();
            control_out = Scope.CreateBus<IndexControl>();
            var pipe0_control = Scope.CreateBus<IndexControl>();
            var pipe1_control = Scope.CreateBus<IndexControl>();
            var pipe2_control = Scope.CreateBus<IndexControl>();

            // Stage 0 - Generate addresses
            var mean_ind = new SLAIndex(control_in, mean_index_A, mean_index_B, pipe0_control, testcontrol, flag_0);

            // Stage 1 - Read from RAM
            var generate_mean = new Generate(mean_index_A, ram_in.ReadControl);
            var pipe_mean1 = new Pipe(mean_index_B, pipeout0_mean);
            var pipe_con1 = new Pipe_control(pipe0_control, pipe1_control);
            var pipe_flag1 = new Pipe_flag(flag_0, flag_1);

            // Stage 2 - Add / count
            var meancount = new Mean_count(pipeout0_mean, mean_result_0, flag_1);
            var meanadd= new Mean_add(ram_in.ReadResult, pipeout0_mean, mean_result_1, flag_1);
            var pipe_mean2 = new Pipe(pipeout0_mean, pipeout1_mean);
            var pipe_con2 = new Pipe_control(pipe1_control, pipe2_control);
            var pipe_flag2 = new Pipe_flag(flag_1, flag_2);

            // Stage 3 - Division
            var meandiv = new Mean_div(mean_result_1, mean_result_0, pipeout1_mean, flag_2, mean_result_2);
            var pipe_con3 = new Pipe_control(pipe2_control, control_out);
            var should_save = new ShouldSave(pipeout1_mean, flag_2, pipeout2_mean);

            // Stage 4 - Save to RAM
            ram_out = new SME.Components.SimpleDualPortMemory<double>(1);
            var toram_mean = new ToRam(mean_result_2, pipeout2_mean, ram_out.WriteControl);

            // var mean_ind = new SigIndex(control_in, mean_index_A ,control_out,  testcontrol);
        }
    }
}