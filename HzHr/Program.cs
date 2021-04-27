using System;
using System.Linq;
using SME;
using SME.Components;
using Deflib;

namespace HzHr
{

    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var sim = new Simulation())
            {
                ////// Test data Hz///////////////

                //busses
                var control_matmul = Scope.CreateBus<IndexControl>();
                var control_prelu_z = Scope.CreateBus<IndexControl>();
                var control_Hz = Scope.CreateBus<IndexControl>();

                //generate flat data
                var matmul_data = Generate_matmul.Matmul_generate();

                //putting iN array
                var array_matmul = new SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size, Deflib.Functions.Flatten(matmul_data));
                var array_prelu_z = new SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks, Deflib.data.prelu_z_slopes);

                //generate expected data
                var hz_expected = Deflib.Functions.Flatten(Deflib.Generate_data.generate_hz(matmul_data, Deflib.dataMatrix.x).Item1);

                //sizes of matrices used
                var index_hz = new TestIndexSim(control_Hz, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size, 1);
                var matmul_ready = new TestIndexSim(control_matmul, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size, 1);
                var prelu_z_ready = new TestIndexSim(control_prelu_z, (int)Deflib.Parameters.num_networks);

                //SME calculation
                var hz = new HzStage(control_Hz, control_matmul, control_prelu_z, array_matmul, array_prelu_z);

                //test out simulation to see if SME match ML results
                var outsim_hz = new OutputSim(hz.control_out, hz.ram_out, hz_expected);

                ////// Test data Hr///////////////
                var control_prelu_r = Scope.CreateBus<IndexControl>();

                var control_hr = Scope.CreateBus<IndexControl>();

                //putting iN array
                var array_prelu_r = new SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks, Deflib.data.prelu_r_slopes);

                //generate expected data
                var hr_expected = Deflib.Functions.Flatten(Deflib.Generate_data.generate_hz(matmul_data, Deflib.dataMatrix.x).Item1);

                //sizes of matrices used
                var index_hr = new TestIndexSim(control_hr, (int)Deflib.Parameters.num_networks, (int)Deflib.Parameters.hidden_size, 1);
                var prelu_r_ready = new TestIndexSim(control_prelu_r, (int)Deflib.Parameters.num_networks);

                //SME calculation
                var hr = new HzStage(control_hr, control_matmul, control_prelu_r, array_matmul, array_prelu_r);

                //test out simulation to see if SME match ML results
                var outsim_hr = new OutputSim(hr.control_out, hr.ram_out, hr_expected);

                sim
                    .BuildCSVFile()
                    .BuildGraph()
                    .Run(exitMethod: () =>
                        {
                            OutputSim[] procs = { outsim_hz, outsim_hr };
                            return procs.All(x => ((IProcess)x).Finished().IsCompleted);
                        }
                    );
            }
        }
    }

    public class HzStage
    {
        public SimpleDualPortMemory<double> ram_out;
        public IndexControl control_out;

        public HzStage(IndexControl testcontrol, IndexControl controlA_in, IndexControl controlB_in, SimpleDualPortMemory<double> ramA_in, SimpleDualPortMemory<double> ramB_in)
        {
            var hz_index_A = Scope.CreateBus<IndexValue>();
            var hz_index_B = Scope.CreateBus<IndexValue>();

            var pipeout_hz = Scope.CreateBus<IndexValue>();
            var pipe1out_hz = Scope.CreateBus<IndexValue>();
            var hz_result = Scope.CreateBus<ValueTransfer>();

            control_out = Scope.CreateBus<IndexControl>();

            ram_out = new SimpleDualPortMemory<double>((int)Deflib.Parameters.num_networks * (int)Deflib.Parameters.hidden_size);

            var generateHzA = new Generate(hz_index_A, ramA_in.ReadControl);
            var generateHzB = new Generate(hz_index_B, ramB_in.ReadControl);

            var hz_ind = new HzIndex(controlA_in, controlB_in, hz_index_A, hz_index_B, control_out, testcontrol);
            var pipe_hz = new Pipe(hz_index_A, pipeout_hz);
            var hz = new Hz(pipeout_hz, ramA_in.ReadResult, ramB_in.ReadResult, hz_result);
            var pipe_hz_1 = new Pipe(pipeout_hz, pipe1out_hz);

            var toram_hz = new ToRam(hz_result, pipe1out_hz, ram_out.WriteControl);
        }
    }

}